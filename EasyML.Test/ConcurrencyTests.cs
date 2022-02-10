using EasyML.Regression;
using EasyML.Test.Models;
using EasyML.Test.Regression;
using NUnit.Framework;
using System;
using System.Linq;

namespace EasyML.Test
{
	[TestFixture]
	internal class ConcurrencyTests
	{
		[TestCase(10)]
		public void GetPredictions_WhileSystemIsTraining(int trainingTimeInSeconds)
		{
			//Arrange
			var configuration = RegressionSystemFixture.GetMinimalConfiguration();
			configuration.MaxTrainingTime = TimeSpan.FromSeconds(trainingTimeInSeconds);
			configuration.EvaluationSetPercentage = 0.5;

			var sut = RegressionSystem<DataRowA>.Create(configuration);
			var dataset = RegressionSystemFixture.GetRandomTrainingData(100);
			
			//First training
			var trainingResult = sut.TrainAsync(dataset).Result;
			Assert.AreEqual(true, trainingResult.Result, "Unexpected result");

			//Test
			var newDataset = RegressionSystemFixture.GetRandomTrainingData(500);
			var task = sut.UpdateAndTrainAsync(newDataset);

			var rnd = new Random();
			while(!task.IsCompleted)
			{
				var question = dataset.ElementAt(rnd.Next(0, dataset.Count()-1)); //Pick a random row from first dataset
				var result = sut.Predict(question); //Get prediction	
			}

			Assert.AreEqual(true, task.Result.Result, "Unexpected result");
		}
	}
}
