using System.Collections.Generic;

namespace EasyML
{
	/// <summary>
	/// Represents the result of a training
	/// </summary>
	public class TrainingResult: TaskResult<bool>
	{
		public string SelectedAlgorithm { get; set; } //TODO:
		public IEnumerable<string> TestedAlgorithms { get; set; }
	}
}
