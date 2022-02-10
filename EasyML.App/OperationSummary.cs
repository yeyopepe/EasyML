using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyML.App
{
	internal class OperationSummary
	{
		public Single Calls { get; set; }
		public string DestinationIP { get; set; }
		public Single AmountOfData { get; set; }
		public Single Weekday { get; set; }
		public bool Result { get; set; }
		//public TimeOnly Daytime { get; set; }
		public Single TotalSeconds { get; set; }
	}
}
