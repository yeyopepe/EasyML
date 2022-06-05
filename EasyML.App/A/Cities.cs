using EasyML.Regression;

namespace EasyML.App.A
{
	internal class Cities
	{
		public static async Task Start()
		{
			var config = new Configuration<Row>("Value");
			var ml = RegressionSystem<Row>.Create(config);

			var dataset = await GetDataset(@".\A\Big_Cities_Health_Data_Inventory.csv");

			var result = await ml.TrainAsync(dataset);

			if (result.Result)
			{
				var q = new Row
				{
					Category = "",
					Gender = "",
					Indicator = "",
					Place ="",
					Race = "",
					Value = 0,
					Year = 0
				};
				Console.WriteLine(ml.Predict(q).Score);
			}
		}

		private static async Task<IEnumerable<Row>> GetDataset(string datasetPath, string separator = ",")
		{
			var result = new List<Row>();
			var content = await File.ReadAllLinesAsync(datasetPath);

			foreach (var row in content.Skip(1))
			{
				var temp = row.Split(separator);
				result.Add(new Row
				{
					Category = temp.ElementAt(0),
					Indicator = temp.ElementAt(1),
					Year = Convert.ToSingle(temp.ElementAt(2)),
					Gender = temp.ElementAt(3),
					Race = temp.ElementAt(4),
					Value = Convert.ToSingle(temp.ElementAt(5)),
					Place = temp.ElementAt(6)
				});
			}

			return result;
		}
	}
}
