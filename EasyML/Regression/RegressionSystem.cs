using EasyML.Exceptions;
using EasyML.Models;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyML.Regression
{
	/// <summary>
	/// Implements a machine learning system based on regression algorithms
	/// </summary>
	/// <typeparam name="TData">System's data type</typeparam>
	public sealed class RegressionSystem<TData> : IMLSystem<TData, Prediction>
		where TData : class
	{
		/// <summary>
		/// Minimal number of dataset's rows to train system
		/// </summary>
		public const int MIN_DATASET_ROWS = 5;

		/// <summary>
		/// Current ML context
		/// </summary>
		private readonly MLContext _context;

		/// <inheritdoc/>
		public Configuration<TData> Configuration { get; private set; }
		/// <inheritdoc/>
		public IEnumerable<TData> TrainingSet { get; private set; }
		/// <inheritdoc/>
		public IEnumerable<TData> EvaluationSet { get; private set; }

		private ITransformer? TrainedModel { get; set; }
		private IDataView TransformedTrainingSet { get; set; }
		private IDataView TransformedEvaluationSet { get; set; }
		private Stream SavedModel { get; set; }

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="configuration">System's configuration</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		private RegressionSystem(Configuration<TData> configuration)
		{
			CheckTData();

			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_context = new MLContext();

			TrainingSet = new List<TData>();
			EvaluationSet = new List<TData>();
		}
		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="savedModel">Saved model</param>
		/// <param name="configuration">System's configuration</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		private RegressionSystem(Stream savedModel,
								Configuration<TData> configuration)
			: this(configuration)
		{
			SavedModel = savedModel ?? throw new ArgumentNullException(nameof(savedModel));

			try
			{
				TrainedModel = _context.Model.Load(savedModel, out _);
			}
			catch (Exception ex)
			{
				throw new ModelNotValidException($"Model content is not valid. More information: {ex.Message}", ex);
			}

		}

		/// <summary>
		/// Checks if TData structure is valid
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		private void CheckTData()
		{
			foreach (var prop in typeof(TData).GetProperties())
			{
				if (prop.PropertyType != typeof(string) &&
					prop.PropertyType != typeof(bool) &&
					prop.PropertyType != typeof(Single))
					throw new ArgumentException($"Type of property {prop.Name} is not supported. Only string, bool and Single are allowed.");
			}
		}

		#region Train
		/// <inheritdoc/>
		public async Task<TrainingResult> TrainAsync(IEnumerable<TData> dataset)
		{
			if (dataset == null || !dataset.Any())
				throw new ArgumentNullException(nameof(dataset));
			if (dataset.Count() < MIN_DATASET_ROWS)
				throw new ArgumentOutOfRangeException(nameof(dataset));

			var trainingSetSize = Convert.ToInt32(dataset.Count() * (1 - Configuration.EvaluationSetPercentage));
			TrainingSet = dataset.Take(trainingSetSize);
			EvaluationSet = dataset.Skip(trainingSetSize);
			return await GeneratedTrainedModel(dataset);

		}
		/// <inheritdoc/>
		public async Task<TrainingResult> UpdateAndTrainAsync(params TData[] newData)
		{
			if (newData == null || !newData.Any())
				throw new ArgumentNullException(nameof(newData));

			TrainingSet = TrainingSet.Concat(newData);
			return await GeneratedTrainedModel(TrainingSet);
		}

		/// <summary>
		/// Trains, evaluates and stores a trained model
		/// </summary>
		/// <param name="dataset">A dataset to train the system. Minimal number of rows: <see cref="MIN_DATASET_ROWS"/></param>
		/// <returns>Training result</returns>
		private async Task<TrainingResult> GeneratedTrainedModel(IEnumerable<TData> dataset)
		{
			var result = new TrainingResult();

			RegressionResult regression = null;
			var step = "Training system...";
			try
			{
				regression = await CreateRegression(trainingData: _context.Data.LoadFromEnumerable(dataset),
													maxTrainingTimeInSeconds: Configuration.MaxTrainingTimeInSeconds);

				result.TestedAlgorithms = regression.TestedAlgorithms;
				result.SelectedAlgorithm = regression.SelectedAlgorithm;

				step = "Evaluating trained model...";
				TrainedModel = Evaluate(EvaluationSet, regression.Result);
				step = "Saving trained model...";
				SavedModel = Export();
			}
			catch (Exception ex)
			{
				result.Error = new MLEngineException($"Error in step '{step}'. More information: {ex.Message}", ex);
			}

			result.Result = regression.Result != null &&
							TrainedModel != null &&
							result.Error == null;

			return result;
		}

		private async Task<RegressionResult> CreateRegression(IDataView trainingData,
																uint maxTrainingTimeInSeconds)
		{
			TransformedTrainingSet = trainingData;

			//ConsoleHelper.ShowDataViewInConsole(mlContext, trainingDataView);

			// STEP 2: Initialize our user-defined progress handler that AutoML will 
			// invoke after each model it produces and evaluates.
			//var progressHandler = new RegressionProgressHandler();

			// STEP 3: Run AutoML regression experiment
			//ConsoleHelper.ConsoleWriteHeader("=============== Training the model ===============");
			//Console.WriteLine($"Running AutoML regression experiment for {maxTrainingTimeInSeconds} seconds...");
			return await Task.Factory.StartNew(() =>
			{
				var result = new RegressionResult();
				try
				{
					var regression = _context.Auto()
						.CreateRegressionExperiment(maxTrainingTimeInSeconds)
						.Execute(TransformedTrainingSet, labelColumnName: Configuration.PredictionColumnName);//, progressHandler: progressHandler);

					result.Result = regression;
					result.TestedAlgorithms = GetTrainersByRSquared(regression);
					result.SelectedAlgorithm = regression.BestRun.TrainerName;
				}
				catch (Exception ex)
				{
					result.Error = ex;
				}

				return result;
			});
		}

		private IEnumerable<Trainer> GetTrainersByRSquared(ExperimentResult<RegressionMetrics> experimentResult)
		{
			var topRuns = experimentResult.RunDetails
				.Where(r => r.ValidationMetrics != null && !double.IsNaN(r.ValidationMetrics.RSquared))
				.OrderByDescending(r => r.ValidationMetrics.RSquared);

			var result = new List<Trainer>(topRuns.Count());

			for (var i = 0; i < topRuns.Count(); i++)
			{
				var run = topRuns.ElementAt(i);
				result.Add(run.ValidationMetrics.GetTrainerInfo(run.TrainerName, run.RuntimeInSeconds));
			}

			return result;
		}
		private ITransformer Evaluate(IEnumerable<TData> testData,
										ExperimentResult<RegressionMetrics> model)
		{
			IDataView testDataView = _context.Data.LoadFromEnumerable(testData);

			RunDetail<RegressionMetrics> best = model.BestRun;
			ITransformer trainedModel = best.Model;
			TransformedEvaluationSet = trainedModel.Transform(testDataView);

			var answerObj = new Prediction();
			var metrics = _context.Regression.Evaluate(TransformedEvaluationSet, labelColumnName: Configuration.PredictionColumnName, scoreColumnName: nameof(answerObj.Score));

			return trainedModel;
		}
		#endregion Train

		/// <inheritdoc/>
		public Prediction Predict(TData question)
		{
			if (question == null)
				throw new ArgumentNullException(nameof(question));
			if (_context == null ||
				TrainedModel == null)
				throw new SystemNotTrainedException();

			var predEngine = _context.Model.CreatePredictionEngine<TData, Prediction>(TrainedModel);

			return predEngine.Predict(question);
		}


		#region Export
		/// <inheritdoc/>
		public Stream Export()
		{
			return Export(() =>
			{
				var result = new MemoryStream();
				_context.Model.Save(TrainedModel, TransformedTrainingSet.Schema, result);
				return result;
			});
		}
		/// <inheritdoc/>
		public void Export(string savedModelPath)=> Export().Save(savedModelPath);

		private T Export<T>(Func<T> exportFunc)
		{
			ValidateExport();
			return exportFunc();
		}
		private void ValidateExport()
		{
			if (TrainedModel == null ||
				_context == null ||
				TransformedTrainingSet == null)
				throw new SystemNotTrainedException();
		}
		#endregion Export

		#region Factory
		/// <summary>
		/// Creates a new machine learning system
		/// </summary>
		/// <param name="configuration">System's configuration</param>
		/// <returns>Machine learning system</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public static RegressionSystem<TData> Create(Configuration<TData> configuration)
			=> new RegressionSystem<TData>(configuration: configuration);

		/// <summary>
		/// Creates a new machine learning system from a previously saved trained model. This methods does not restore the data, only the trained model to ask for predictions
		/// </summary>
		/// <param name="savedModel">Saved model</param>
		/// <param name="configuration">System's configuration</param>
		/// <returns>Machine learning system</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public static RegressionSystem<TData> Load(Stream savedModel, Configuration<TData> configuration)
			=> new RegressionSystem<TData>(savedModel: savedModel, configuration: configuration);

		/// <summary>
		/// Creates a new machine learning system from a previously saved trained model. This methods does not restore the data, only the trained model to ask for predictions
		/// </summary>
		/// <param name="saveModelPath">Complete path to saved model</param>
		/// <param name="configuration">System's configuration</param>
		/// <returns>Machine learning system</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="Exception"></exception>
		public static RegressionSystem<TData> Load(string saveModelPath, Configuration<TData> configuration)
		{
			if (!File.Exists(saveModelPath))
				throw new FileNotFoundException(nameof(saveModelPath));

			var str = new FileStream(saveModelPath, FileMode.Open);

			return new RegressionSystem<TData>(savedModel: str,
										configuration: configuration);
		}
		#endregion Factory
	}
}
