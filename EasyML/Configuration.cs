using System;

namespace EasyML
{
	/// <summary>
	/// Represents the configuration of a ML system
	/// </summary>
	/// <typeparam name="TData">System's data type</typeparam>
	public class Configuration<TData>
		where TData : class
	{
		/// <summary>
		/// Default training time
		/// </summary>
		public readonly TimeSpan DEFAULT_TRAINING_TIME = TimeSpan.FromSeconds(20);
		/// <summary>
		/// Default percentage to split given datasets and get testing data
		/// </summary>
		public readonly double DEFAULT_EVALUATION_PERCENTAGE = 0.25;

		private double _evaluationSetPercentage;

		/// <summary>
		/// Gets or sets the max time a system can train
		/// </summary>
		public TimeSpan MaxTrainingTime { get; set; }
		/// <summary>
		/// Gest or sets the percentage to split given datasets and get testing data
		/// </summary>
		public double EvaluationSetPercentage
		{
			get
			{
				return _evaluationSetPercentage;
			}
			set
			{
				if (_evaluationSetPercentage <= 0 ||
					_evaluationSetPercentage > 1)
					_evaluationSetPercentage = DEFAULT_EVALUATION_PERCENTAGE;

				_evaluationSetPercentage = value;
			}
		}
		/// <summary>
		/// Gets or sets the column's name from TData to predict its value
		/// </summary>
		public string PredictionColumnName { get; private set; }

		/// <summary>
		/// Gets or sets the the max time a system can train in seconds
		/// </summary>
		internal uint MaxTrainingTimeInSeconds
		{
			get
			{
				if (MaxTrainingTime == null ||
					MaxTrainingTime.TotalSeconds < 1)
					return (uint)DEFAULT_TRAINING_TIME.TotalSeconds;

				return (uint)MaxTrainingTime.TotalSeconds;
			}
		}

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="predictionColumnName">Column's name from TData to predict its value</param>
		public Configuration(string predictionColumnName)
		{
			CheckPredictionColumnName(predictionColumnName);
			PredictionColumnName = predictionColumnName;
		}

		/// <summary>
		/// Checks if given column name is valid and coherent with TData
		/// </summary>
		/// <param name="predictionColumnName">Column's name from TData to predict its value</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		private void CheckPredictionColumnName(string predictionColumnName)
		{
			if (string.IsNullOrWhiteSpace(predictionColumnName))
				throw new ArgumentNullException(nameof(predictionColumnName));

			if (typeof(TData).GetProperty(predictionColumnName) == null)
				throw new ArgumentException($"Column {nameof(predictionColumnName)} does not exist in {typeof(TData).Name}");
		}
	}
}
