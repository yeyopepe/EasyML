using System;

namespace EasyML
{
	/// <summary>
	/// Represents a generic response given by a task
	/// </summary>
	/// <typeparam name="T">Result type</typeparam>
	public class TaskResult<T>
	{
		/// <summary>
		/// Gets the error found while task was running (if any)
		/// </summary>
		public Exception Error { get; set; }
		/// <summary>
		/// Gets or sets the result of the task
		/// </summary>
		public T Result { get; set; }
	}
}
