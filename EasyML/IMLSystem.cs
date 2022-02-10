using EasyML.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EasyML
{
	/// <summary>
	/// Represents a machine learning system
	/// </summary>
	/// <typeparam name="TData">System's data type</typeparam>
	/// <typeparam name="TPrediction">System's prediction data type</typeparam>
	public interface IMLSystem<TData, TPrediction> 
		where TData : class
		where TPrediction : class
	{
		/// <summary>
		/// Gets the system configuration
		/// </summary>
		Configuration<TData> Configuration { get; }
		/// <summary>
		/// Gets the current evaluation dataset the system was be trained with
		/// </summary>
		IEnumerable<TData> EvaluationSet { get; }
		/// <summary>
		/// Gets the current training dataset the system was be trained with
		/// </summary>
		IEnumerable<TData> TrainingSet { get; }

		/// <summary>
		/// Trains the system from scratch creating a new trained model
		/// </summary>
		/// <param name="dataset">A dataset to train the system. Minimal number of rows: <see cref="MIN_DATASET_ROWS"/></param>
		/// <returns>Training result</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		Task<TrainingResult> TrainAsync(IEnumerable<TData> dataset);
		/// <summary>
		/// Adds new data to the previously trained system and trains it again
		/// </summary>
		/// <param name="newData">New data to update the system and train again</param>
		/// <returns>New training result</returns>
		/// <exception cref="ArgumentNullException"></exception>
		Task<TrainingResult> UpdateAndTrainAsync(IEnumerable<TData> newData);

		/// <summary>
		/// Predicts a value given a question 
		/// </summary>
		/// <param name="question">A row of data</param>
		/// <returns>Predicted value</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="SystemNotTrainedException"></exception>
		TPrediction Predict(TData question);
		/// <summary>
		/// Exports the last trained model
		/// </summary>
		/// <returns>Last trained model</returns>
		Stream Export();
	}
}