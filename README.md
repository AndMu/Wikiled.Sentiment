# *pSenti* Sentiment Analysis Library (C#)

Nuget library

![nuget](https://img.shields.io/nuget/v/Wikiled.Sentiment.Analysis.svg)

```
Install-Package Wikiled.Sentiment
```

## Standalone application - *pSenti*

### To test using lexicon based model 
```
Wikiled.Sentiment.ConsoleApp.exe test -Out=[OutPut] -Input=[Folder/File]
Wikiled.Sentiment.ConsoleApp.exe test -Out=[OutPut] -Positive=[Folder/File] -Negative=[Folder/File]
```

### To override lexicon
```
Wikiled.Sentiment.ConsoleApp.exe test -Out=[OutPut] -Input=[Folder/File] -Weights[WeightsFile] -FullWeightReset
```

### To train with non default lexicon
```
Wikiled.Sentiment.ConsoleApp.exe train -Positive=[Folder/File] -Negative=[Folder/File] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset] -Model=[Path to Model]
```

### To Test with trained model
```
Wikiled.Sentiment.ConsoleApp.exe test -Out=[OutPut] -Input=[Folder/File] -Model=[Path to Trained Model]
```
## Linux support

[Supported OS](https://github.com/dotnet/core/blob/master/release-notes/2.0/2.0-supported-os.md)

* Install [dotnet core](https://www.microsoft.com/net/download/)
* Retrieve GIT repository source
* dotnet build src/Utilities/Wikiled.Sentiment.ConsoleApp --configuration Release
* dotnet src/Utilities/Wikiled.Sentiment.ConsoleApp/bit/Release/netcoreapp2.0/Wikiled.Sentiment.ConsoleApp.dll test -Articles=test -Input=lexicon.csv -out=Result -ExtractStyle]

## C# Library 

## Resources

```
<appSettings>
<add key="resources" value="Resources" />
</appSettings>
```

### Training model

```
ICacheFactory cacheFactory = new LocalCacheFactory();
IObservable<IParsedDocumentHolder> reviews = GetReviews();
var localCache = new LocalCacheFactory();
var splitterHelper = new MainSplitterFactory(localCache, configuration).Create(POSTaggerType.SharpNLP);
ProcessingPipeline pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, splitterHelper, reviews, new ParsedReviewManagerFactory());
TrainingClient trainingClient = new TrainingClient(pipeline, @".\Model");
			
await client.Train();
```

### Testing model

```
client = new TestingClient(pipeline, Trained);
client.Init();
await client.Process();
client.Save(Out);
```
