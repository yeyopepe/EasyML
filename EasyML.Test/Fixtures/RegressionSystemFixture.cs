using EasyML.Test.Fixtures.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyML.Test.Fixtures
{
    internal class RegressionSystemFixture
	{
        public static IEnumerable<DataRowA> GetRandomTrainingData(int count)
        {
            var rnd = new Random();
            var letter = "";
            return Enumerable.Range(1, count)
                .Select(i =>
                new DataRowA
                {
                    Type = (letter = new string[] { "a", "b", "c" }.ElementAt(rnd.Next(0, 3))),
                    Other = DateTime.Now.Millisecond,
                    Value = rnd.Next(0, 20)
                });
        }

        public static Configuration<DataRowA> GetMinimalConfiguration()
		{
            var temp = new DataRowA();
            return new Configuration<DataRowA>(nameof(temp.Value));
        }
    }
}
