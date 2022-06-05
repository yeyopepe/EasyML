# EasyML
EasyML is a component to train a machine learning system and use it to get predictions with just a few lines of code. The objective of this project is to facilitate the use of ML algorithms in any project by any developer with almost no knowledge about ML. It is basically a wrapper **to allow devs to focus only in get advantage from this technology as fast as possible** instead of getting lost themselves in the implementation details.

 

## Features
- Regression implementation: *A supervised machine learning implementation that is used to predict the value of a column (label) from a set of related features. The label can be of any real value.*
- Async/parallel training in order to get your system always working.
- Automatic selection of algorithm depending on your data and time your system expends training.


## Basic workflow
1. Define and get a dataset to train your system.
2. Configure and instantiate.
3. Train it using your dataset (to get a trained model).
4. Ask for a prediction.
5. Also: you can update your system with new data and train it again anytime while you continue asking for predictions. When the system finishes their training the new predictions will use the new trained model automatically.



## 👉 How to use

### 1 - Model your data
The first thing you must define is the data you will use to train your system and what are que predictions you want to get.

Suppose we need a ML system to predict the estimated time a expensive operation lasts. I can get the following data every time I performed one of those operations (that are the parameters/features of your system): *Number of requests you operation performs, Server we call, Total amount of sent data (in Kb), weekday (1-7) and Total seconds (this is the parameter I want predict)*.

Write a class to work with your data:
```
class OperationSummary
{
	public Single Requests { get; set; }
	public string DestinationIP { get; set; }
	public Single AmountOfData { get; set; }
	public Single Weekday { get; set; }
	public Single TotalSeconds { get; set; }
}
```


### 2 - Configure the system
You need to create a new Configuration telling the type of data your system will work with (the previous OperationSummary summary) and the name of the column the system must predict. 
```
var configuration = new EasyML.Configuration<OperationSummary>("TotalSeconds");
```
You can also tune some other things (like the max time you allow your system to be trained). If you do not specify any value the system use the default ones.

After that you can get a instance of your system:
```
var ml = RegressionSystem<OperationSummary>.Create(configuration);
```


### 3 - Get a starting dataset and train your system
In order to train your system for very first time you need to get an initial dataset (minimum 5 rows). You can store it in a file and parse it or whatever but at the end you need an IEnumerable with at least 5 rows.

```
IEnumerable<OperationSummary> dataSet = GetYourDataset();
```

Now you can start to train your system: just call TrainAsync method, do whatever your want and wait for the task:

```
var task = ml.TrainAsync(dataset); //This returns a Task you can wait
task.Wait();

if (task.Result.Result)
{
	//Your system is trained and ready
}
else
{
	//Check result.Error to get information about the error
}
```

Or if your write properly async code you can await it:

```
var task = await ml.TrainAsync(dataset);

if (task.Result) 
{ //OK }
...
```


### 4 - Ask for a prediction
Now you have a trained system you can ask for any prediction. Suppose your receive a new request to perform a new operation. Just ask for a prediction (TotalSeconds, the column you configured previously) and *voilà*:
```
var request = new OperationSummary
{
	AmountOfData = 10000, //10Mb
	Requests = 5, 
	Weekday = 1, //Monday
	DestinationIP = "192.168.1.1",
	TotalSeconds = 0 //Value to predict
};

var estimatedTotalSeconds = ml.Predict(request);
```

### 5 - Improve your system
Every time you finished a new operation (a get the real duration) you can use your new record to improve your system. You can collect those data and train your system again in order to improve its future results. The way to do that is the same but using the method UpdateAndTrainAsync() in order to tell your system that it has to add the new data to the previous one.

```
var task = ml.UpdateAndTrainAsync(newData); //A new task is triggered
```

❗**Remember:**
- *TrainAsync()* restarts your system and trains it from scratch.
- *UpdateAndTrainAsync()* keep your current system and add new data.


## 👉 Advanced tips
### Take a glimpse under the hood
Every time you train a system you can see some information about the 
```
var trainingResult = ml.TrainAsync(dataset).Result;

Console.WriteLine("Selected: " + trainingResult.SelectedAlgorithm);
Console.WriteLine("Tested" + String.Join(trainingResult.SelectedAlgorithm, ", "));
```

### Preserve your trained model
There is several ways to preserve your trained model and recover your system in case of trouble.

You can export your current trained model as a stream to store it wherever:
```
var savedModel = ml.Export(); //Gets a stream

//OR

var path = @"c:\data\model.zip";
ml.Export(path); //Saves your trained model into a file
```

Of course you alsco can load your trained model in order to restore your system:
```
var ml = RegressionSystem<OperationSummary>.Load(savedModelSteam, configuration); //Stream

//OR

var ml = RegressionSystem<OperationSummary>.Load(savedModelPath, configuration); //File
```

❗**Remember:** Load() method recovers your trained model so you can continue ask for predictions. This method DOES NOT recover the data the system used to be trained so your system never can be improved.



## About performance
- ML systems can consume a lot of time being trained (depending on how precise you want to be and how long you let it be trained). This operation can be done in parallel...

## Roadmap

Feature										|Implemented|Version
|-------------------------------------------|:---------:|:--------------:
|Regression									|✅         |1.0.0
|Export/Load trained model				|✅         |1.0.0
|Export/Load trained model to/from files			|✅         |1.0.3
|Other implementations like multiclass classification, clustering, anomaly detection, etc..  |❌         |



## Credits and more information
This project uses Microsoft's *Microsoft.ML.AutoML* package, an implementation to create, train, evaluate and get predictions from ML models. 

- ML.NET: https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet
- About available algorithms: https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-choose-an-ml-net-algorithm
