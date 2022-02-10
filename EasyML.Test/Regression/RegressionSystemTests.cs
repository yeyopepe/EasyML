using EasyML.Exceptions;
using EasyML.Regression;
using EasyML.Test.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyML.Test.Regression
{
	public class RegressionSystemTests
	{
		[Test]
		public void TrainAsync_DatasetAsEnumerable_ReturnsValidDatasets()
		{
			//Arrange
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();

			var sut = RegressionSystem<DataRowA>.Create(configuration);
			var dataset = RegressionSystemFixture.GetRandomTrainingData(100);

			//Test
			_ = sut.TrainAsync(dataset).Result;

			//Assert
			var expectedEvaluationDataSetSize = (int)dataset.Count() * configuration.EvaluationSetPercentage;
			var expectedTrainingDataSetSize = dataset.Count() - expectedEvaluationDataSetSize;

			Assert.AreEqual(expectedTrainingDataSetSize, sut.TrainingSet.Count(), "Training dataset");
			Assert.AreEqual(expectedEvaluationDataSetSize, sut.EvaluationSet.Count(), "Evaluation dataset");
		}

		[TestCase(2u, 10)]
		[TestCase(2u, 100)]
		[TestCase(10u, 100)]
		public void TrainAsync_DatasetAsEnumerable_ReturnsTrue(uint trainingTimeInSeconds, int datasetSize)
		{
			//Test
			var dataset = RegressionSystemFixture.GetRandomTrainingData(datasetSize);
			Train(dataset: dataset,
					out RegressionSystem<DataRowA> sut,
					out TrainingResult? trainingResult,
					trainingTimeInSeconds: trainingTimeInSeconds);

			//Assert
			Assert.AreEqual(true, trainingResult.Result, "Unexpected result");
			Assert.IsNull(trainingResult.Error, "Unexpected error");

			var sample = dataset.First();
			var result = sut.Predict(sample);
			Assert.AreNotEqual(sample.Value, result.Score);
		}
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		public void TrainAsync_DatasetAsEnumerable_InvalidDatasetSize_ThrowsException(int datasetSize)
		{
			//Arrange
			var sut = RegressionSystem<DataRowA>.Create(RegressionSystemFixture.GetMinimalConfiguration());
			var dataset = RegressionSystemFixture.GetRandomTrainingData(datasetSize);

			//Test
			var result = Assert.Throws<AggregateException>(() => _ = sut.TrainAsync(dataset).Result);

			Assert.AreEqual(typeof(ArgumentOutOfRangeException), result.InnerException.GetType(), "Unexpected exception");
		}
		[TestCase(true, false)]
		[TestCase(false, true)]
		public void TrainAsync_DatasetAsEnumerable_EmptyDataset_ThrowsException(bool isNull, bool isEmpty)
		{
			//Arrange
			var sut = RegressionSystem<DataRowA>.Create(RegressionSystemFixture.GetMinimalConfiguration());
			var dataset = isNull ?
							null :
							isEmpty ?
								RegressionSystemFixture.GetRandomTrainingData(0) :
								RegressionSystemFixture.GetRandomTrainingData(100);

			//Test
			var result = Assert.Throws<AggregateException>(() => _ = sut.TrainAsync(dataset).Result);

			Assert.AreEqual(typeof(ArgumentNullException), result.InnerException.GetType(), "Unexpected exception");
		}
		[Test]
		public void TrainAsync_DatasetAsEnumerable_ErrorDuringTraining_ReturnsFalse()
		{
			//TODO: No sé cómo testear este caso...
			Assert.Fail();

			//Arrange
			var sut = RegressionSystem<DataRowA>.Create(RegressionSystemFixture.GetMinimalConfiguration());
			var dataset = RegressionSystemFixture.GetRandomTrainingData(10);

			//Test
			var trainingResult = sut.TrainAsync(dataset).Result;

			//Assert
			Assert.AreEqual(false, trainingResult.Result, "Unexpected result");
			Assert.AreEqual(typeof(MLEngineException), trainingResult.Error.GetType(), "Unexpected error");
		}

		[Test]
		public void UpdateAndTrain_DatasetAsEnumerable_ReturnsTrue()
		{
			//Arrange
			var dataset = RegressionSystemFixture.GetRandomTrainingData(50);
			Train(dataset: dataset,
					out RegressionSystem<DataRowA> sut,
					out TrainingResult? trainingResult,
					trainingTimeInSeconds: 3);


			//Store a sample and prediction
			var sample = dataset.First();
			var result = sut.Predict(sample);

			//Test
			//We need a new dataset with lots of identical cases to introduce bias in current model
			var newDataSet = Enumerable.Range(0, 400)
				.Select(i =>
				new DataRowA()
				{
					Type = sample.Type,
					Other = sample.Other,
					Value = sample.Value
				});
			var trainingResult2 = sut.UpdateAndTrainAsync(newDataSet).Result;
			Assert.AreEqual(true, trainingResult.Result, "Unexpected result");
			Assert.IsNull(trainingResult.Error, "Unexpected error");

			//Assert
			var result2 = sut.Predict(sample);
			var errorInFirstPrediction = Math.Abs(sample.Value - result.Score);
			var errorInSecondPrediction = Math.Abs(sample.Value - result2.Score);

			//First prediction must be worse than second one
			Assert.IsTrue(errorInFirstPrediction > errorInSecondPrediction, $"errorInFirstPrediction= {errorInFirstPrediction}, errorInSecondPrediction= {errorInSecondPrediction}");
		}
		[Test]
		public void UpdateAndTrain_DatasetAsEnumerable_EmptyDataset_ThrowsException()
		{
			//Arrange
			var dataset = RegressionSystemFixture.GetRandomTrainingData(50);
			Train(dataset,
					out RegressionSystem<DataRowA> sut,
					out TrainingResult? trainingResult);

			//Test
			var result = Assert.Throws<AggregateException>(() => _ = sut.UpdateAndTrainAsync(null).Result);
			Assert.AreEqual(typeof(ArgumentNullException), result.InnerException.GetType(), "Unexpected exception");
		}

		[Test]
		public void SetConfiguration_ValidPredictionColumnName_ReturnsConfiguration()
		{
			//Test
			var temp = new DataRowA();
			var result = new Configuration<DataRowA>(nameof(temp.Value));

			//Test
			Assert.AreEqual(nameof(temp.Value), result.PredictionColumnName);
		}
		[Test]
		public void SetConfiguration_InvalidPredictionColumnName_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => _ = new Configuration<DataRowA>("invalidColumnName"));
		}

		[Test]
		public void ExportModel_Trained_ReturnsStream()
		{
			//Arrange
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			configuration.MaxTrainingTime = TimeSpan.FromSeconds(1);

			var sut = RegressionSystem<DataRowA>.Create(configuration);
			var dataset = RegressionSystemFixture.GetRandomTrainingData(10);

			//Test
			_ = sut.TrainAsync(dataset).Result;

			var savedModel = sut.Export();

			//Assert
			Assert.IsNotNull(savedModel);
		}
		[Test]
		public void ExportModel_NoTrained_ThrowsException()
		{
			//Arrange
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			configuration.MaxTrainingTime = TimeSpan.FromSeconds(1);

			var sut = RegressionSystem<DataRowA>.Create(configuration);

			//Test
			var result = Assert.Throws<SystemNotTrainedException>(() => _ = sut.Export());
		}

		[Test]
		public void CreateFromModel()
		{
			//Arrange
			//First system
			var dataset = RegressionSystemFixture.GetRandomTrainingData(100);
			Train(dataset: dataset,
					out RegressionSystem<DataRowA> sut1,
					out TrainingResult? trainingResult);

			var savedModel = sut1.Export();

			var sample = dataset.First();
			var predictionSystem1 = sut1.Predict(sample);

			//Test
			//New system loaded from a previous one
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			var sut2 = RegressionSystem<DataRowA>.CreateFromModel(savedModel, configuration);

			var predictionSystem2 = sut2.Predict(sample);

			//Assert
			Assert.AreEqual(predictionSystem1.Score, predictionSystem2.Score, "Unexpected score");
		}
		[Test]
		public void CreateFromModel_EmptySavedModel_ThrowsException()
		{
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			Assert.Throws<ArgumentNullException>(() => _ = RegressionSystem<DataRowA>.CreateFromModel((Stream)null, configuration));
		}
		[Test]
		public void CreateFromModel_InvalidSavedModel_ThrowsException()
		{
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			
			var invalidModel = new MemoryStream();
			var writer = new StreamWriter(invalidModel);
			writer.Write("whatever");
			writer.Flush();
			invalidModel.Position = 0;

			writer.WriteLine("adfasdf");
			
			Assert.Throws<ModelNotValidException>(() => _ = RegressionSystem<DataRowA>.CreateFromModel(invalidModel, configuration));
		}

		[Test]
		public void TryToGetSystem_InvalidDataStructure_ThrowsException()
		{
			//Arrange
			var temp = new InvalidDataStructure();
			var config = new Configuration<InvalidDataStructure>(nameof(temp.Value));

			//Test
			Assert.Throws<ArgumentException>(() => _ = RegressionSystem<InvalidDataStructure>.Create(config));
		}
		private void Train(IEnumerable<DataRowA> dataset,
						out RegressionSystem<DataRowA> sut,
						out TrainingResult? trainingResult,
						uint trainingTimeInSeconds = 1)
		{
			//Arrange
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			configuration.MaxTrainingTime = TimeSpan.FromSeconds(trainingTimeInSeconds);

			sut = RegressionSystem<DataRowA>.Create(configuration);

			trainingResult = sut.TrainAsync(dataset).Result;
		}
	}
}