using Microsoft.ML.Data;
using System;

namespace EasyML.Regression
{
    /// <summary>
    /// Represents a prediction
    /// </summary>
	public class Prediction
    {
        [ColumnName("Score")]
        public Single Score;
    }
}
