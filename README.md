# EasyML
EasyML is a component to train a machine learning system and use it to get predictions with just a few lines of code. The objective of this project is to facilitate the use of ML algorithms in any project by any developer with no knowledge about ML. It is basically a wrapper **to allow devs to focus only in get advantage from this technology as fast as possible** instead of getting lost themselves in the implementation details.

 

## Features
- Regression implementation: *A supervised machine learning implementation that is used to predict the value of a column (label) from a set of related features. The label can be of any real value.*


## How to use
1️⃣
### Model your data
The first thing you must define is the data you will use to train your system and what are que predictions you want to get.

Suppose we need a ML system to predict the estimated time a expensive operation lasts. I can get the following data every time I performed one of those operations (that are the parameters/features of your system): Number of calls needed, Server we call, Total amount of data sent, weekday, daytime, Total seconds .
I write this in a class:
```
class OperationSummary
{
	public int Calls { get; set; }
	public string DestinationIP { get; set; }
	public int AmountOfData { get; set; }
	public int Weekday { get; set; }
	public TimeOnly Daytime { get; set; }
	public uint TotalSeconds { get; set; }
}
```

⚠️**WORK IN PROGRESS...**


## About performance
- ML systems can consume a lot of time being trained (depending on how precise you want to be and how long you let it be trained). This operation can be done in parallel...

## Roadmap
- Save and load datasets from files
- Save and load trained model to/from files
- Other implementations like multiclass classification, clustering, anomaly detection, etc..

## Credits and more information
This project uses Microsoft's *Microsoft.ML.AutoML* package, an implementation to create, train, evaluate and get predictions from ML models. 

- ML.NET: https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet
- Available algorithms: https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-choose-an-ml-net-algorithm
