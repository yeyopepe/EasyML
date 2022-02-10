using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System;

namespace EasyML.Regression
{
    public class RegressionProgressHandler : IProgress<RunDetail<RegressionMetrics>>
    {
        private int _iterationIndex;
        //TODO: List of trainers, results, etc...

        public void Report(RunDetail<RegressionMetrics> iterationResult)
        {
            if (_iterationIndex++ == 0)
            {
                ConsoleHelper.PrintRegressionMetricsHeader();
            }

            if (iterationResult.Exception != null)
            {
                ConsoleHelper.PrintIterationException(iterationResult.Exception);
            }
            else
            {
                ConsoleHelper.PrintIterationMetrics(_iterationIndex, iterationResult.TrainerName,
                    iterationResult.ValidationMetrics, iterationResult.RuntimeInSeconds);
            }
        }
    }
}
