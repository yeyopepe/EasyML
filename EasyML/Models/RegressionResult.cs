using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace EasyML.Models
{
	/// <summary>
	/// Represents the result of a regression
	/// </summary>
	internal class RegressionResult : TaskResultBase<ExperimentResult<RegressionMetrics>>
	{
		/// <summary>
		/// Gets or sets the Selected Algorithm after a training
		/// </summary>
		public string SelectedAlgorithm { get; set; } 
		/// <summary>
		/// Gets or sets the list of algorithms used to train the system
		/// </summary>
		public IEnumerable<Trainer> TestedAlgorithms { get; set; }
	}
}
