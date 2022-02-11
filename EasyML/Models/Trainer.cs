namespace EasyML.Models
{
	/// <summary>
	/// Represents an algorithm used to train the system
	/// </summary>
	public class Trainer
	{
		/// <summary>
		/// Name of the trainer
		/// </summary>
		public string Name { get; internal set; }
		/// <summary>
		/// Gets the R-squared value of the model, which is also known as the coefficient
		///  of determination​. R-Squared closer to 1 indicates a better fitted model.
		/// </summary>
		/// <see cref = "Microsoft.ML.Data.RegressionMetrics.RSquared" />
		public double RSquared { get; internal set; }
		/// <summary>
		/// Gets the absolute loss of the model.
		/// </summary>
		/// <see cref = "Microsoft.ML.Data.RegressionMetrics.MeanAbsoluteError" />
		public double AbsoluteError { get; internal set; }
		/// <summary>
		/// Gets the squared loss of the model.
		/// </summary>
		/// <see cref = "Microsoft.ML.Data.RegressionMetrics.MeanSquaredError" />
		public double SquaredLoss { get; internal set; }
		/// <summary>
		/// Gets the root mean square loss (or RMS) which is the square root of the L2 loss		
		/// </summary>
		/// <see cref = "Microsoft.ML.Data.RegressionMetrics.RootMeanSquaredError" />
		public double RMSLoss { get; internal set; }
		/// <summary>
		/// Time the trainner was running
		/// </summary>
		public double TrainingDurationInSeconds{ get; internal set; }

	}
}
