using Microsoft.ML.Data;

namespace EasyML.Models
{
	internal static class Mapper
	{
		public static Trainer GetTrainerInfo(this RegressionMetrics metrics, string name, double durationInSeconds)
		{
			return new Trainer
			{
				Name = name,
				TrainingDurationInSeconds = durationInSeconds,
				AbsoluteError = metrics.MeanAbsoluteError,
				RMSLoss = metrics.RootMeanSquaredError,
				RSquared = metrics.RSquared,
				SquaredLoss = metrics.MeanSquaredError
			};
		}
	}
}
