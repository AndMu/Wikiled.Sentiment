# *pSenti* Sentiment Analysis Library (C#)

Nuget library

![nuget](https://img.shields.io/nuget/v/Wikiled.Sentiment.Analysis.svg)

```
Install-Package Wikiled.Sentiment
```
## Resources

Both Standalone and library require [resources](src\Resources\..)
In app.config/Wikiled.Sentiment.ConsoleApp.config:

```
<appSettings>
<add key="resources" value="..\..\..\..\Resources" />
<add key="Stanford" value="..\..\..\..\Resources\Stanford" />
<add key="Lexicon" value="Library\Standard" />
</appSettings>
```

## Standalone application - *pSenti*

Supported taggers:
- SharpNLP
- Stanford
- Basic

### To test using lexicon based model 
```
Wikiled.Sentiment.ConsoleApp.exe test -Tagger=SharpNLP -Out=[OutPut] -Input=[Folder/File]
Wikiled.Sentiment.ConsoleApp.exe test -Tagger=SharpNLP -Out=[OutPut] -Positive=[Folder/File] -Negative=[Folder/File]
```

### To override lexicon
```
Wikiled.Sentiment.ConsoleApp.exe test -Tagger=SharpNLP -Out=[OutPut] -Input=[Folder/File] -Weights[WeightsFile] -FullWeightReset
```

### To train with non default lexicon
```
Wikiled.Sentiment.ConsoleApp.exe train -Tagger=SharpNLP -Positive=[Folder/File] -Negative=[Folder/File] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset] -Model=[Path to Model]
```

### To Test with trained model
```
Wikiled.Sentiment.ConsoleApp.exe test -Tagger=SharpNLP -Out=[OutPut] -Input=[Folder/File] -Trained=[Path to Trained Model]
```

## C# Library 

### Training model

```
ICacheFactory cacheFactory = new LocalCacheFactory();
IObservable<IParsedDocumentHolder> reviews = GetReviews();
var splitter = new SplitterFactory(cacheFactory, new ConfigurationHandler()).Create(Tagger);
TrainingClient client = new TrainingClient(splitter, reviews, @".\Model");
await client.Train();
```

### Testing model

```
client = new TestingClient(splitter, reviews, Trained);
client.Init();
await client.Process();
client.Save(Out);
```
