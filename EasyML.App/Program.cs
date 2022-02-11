using EasyML.App;
using EasyML.Regression;


var x = "";
var configuration = new EasyML.Configuration<OperationSummary>("TotalSeconds");
configuration.MaxTrainingTime = TimeSpan.FromSeconds(3);
var ml = RegressionSystem<OperationSummary>.Create(configuration);

var rnd = new Random();
var dataset = Enumerable.Range(0, 100).Select(
	i =>
	new OperationSummary
	{
		AmountOfData = i * 100,
		Calls = rnd.Next(10, 20),
		Weekday = rnd.Next(1, 7),
		DestinationIP = $"{rnd.Next(1, 250)}.{rnd.Next(1, 250)}.{rnd.Next(1, 250)}.{rnd.Next(1, 250)}",
		Result = new bool[2] { true, false }.ElementAt(rnd.Next(0,2)),
		//Daytime = TimeOnly.FromDateTime(DateTime.Now),
		TotalSeconds = rnd.Next(60, 300)
	});

var task = ml.TrainAsync(dataset);
task.Wait();



if (task.Result.Result)
{
	//Your system is trained and ready
}
else
{
	//Check result.Error to get information about the error
}
var score = ml.Predict(dataset.First());
