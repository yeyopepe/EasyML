using System;
using System.IO;

namespace EasyML
{
	internal static class Extensions
	{
		/// <summary>
		/// Saves the given data into a file
		/// </summary>
		/// <param name="data">Data to save</param>
		/// <param name="filePath">Complete path to file</param>
		/// <exception cref="Exception"></exception>
		internal static void Save(this Stream data, string filePath)
		{
			data.Seek(0, SeekOrigin.Begin);

			using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
			{
				data.CopyTo(fs);
			}
		}
	}
}
