using System.Collections.Generic;

namespace EasyML.Models
{
	/// <summary>
	/// Represents the result of a training
	/// </summary>
	public class TrainingResult: TaskResultBase<bool>
	{
		/// <summary>
		/// Gets or sets the Selected Algorithm after a training
		/// </summary>
		public string SelectedAlgorithm { get; internal set; }
		/// <summary>
		/// Gets or sets the list of algorithms used to train the system
		/// </summary>
		public IEnumerable<Trainer> TestedAlgorithms { get; internal set; }
	}
}
