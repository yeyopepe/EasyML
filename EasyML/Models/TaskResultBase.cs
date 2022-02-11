using System;

namespace EasyML.Models
{
	/// <summary>
	/// Represents a generic response given by a task
	/// </summary>
	/// <typeparam name="T">Result type</typeparam>
	public abstract class TaskResultBase<T>
	{
		/// <summary>
		/// Gets the error found while task was running (if any)
		/// </summary>
		public Exception Error { get; internal set; }
		/// <summary>
		/// Gets or sets the result of the task
		/// </summary>
		public T Result { get; internal set; }
	}
}
