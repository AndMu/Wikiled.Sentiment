# *pSenti* Sentiment Analysis Library (C#)

Nuget library

![nuget](https://img.shields.io/nuget/v/Wikiled.Sentiment.Analysis.svg)

```
Install-Package Wikiled.Sentiment
```

## Concept-Level Domain Sentiment Analysis System


To overcome the lexicon and bag-of-words learning limitations, we have developed a sentiment-analysis method which is less sensitive to crossing domain boundaries and has similar performance to pure learning-based methods.
It is based on [research paper](docs/Original.pdf).
Which was later improved in [extended version](docs/extended.pdf)

The main advantage of our *hybrid* approach using a lexicon/learning symbiosis is to get the best of both worlds - the stability and readability of a carefully hand-picked lexicon, and the high accuracy from a powerful supervised learning algorithm.
Thanks to the built-in sentiment lexicon and numerous linguistic rules, **pSenti** can detect and measure sentiments at the concept level, providing structured and readable aspect-oriented outputs

The core of **pSenti** is its lexicon-based system, so it shares many common NLP processing techniques with other similar approaches.
In our proposed model, the learning phase is responsible for the lexicon part of domain adaptation by adjusting sentiment word values and participating in a domain-specific lexicon expansion.

A classic bag-of-words supervised learning approach takes all words as features; however, not all words carry sentiment information.
Thus, if we limit features only to well-known and potential sentiment words, we would be able to use weights extracted from a linear SVM to learn their importance and impact on a sentiment classification task.
In other words, we would discover their domain-specific sentiment values.
Due to normalisation and standardisation, feature weights discovered in the training phase are on the same scale, and their interpretation can be mapped back into the lexicon-based part and represent their domain-specific sentiment strength.

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

## Docker service

An application is also available as a standalone docker based REST service, available in [GitHub](AndMu/Wikiled.Sentiment.Service).

## Linux support

[Supported OS](https://github.com/dotnet/core/blob/master/release-notes/2.0/2.0-supported-os.md)

* Install [dotnet core](https://www.microsoft.com/net/download/)
* Retrieve GIT repository source
* dotnet build src/Utilities/Wikiled.Sentiment.ConsoleApp --configuration Release
* dotnet src/Utilities/Wikiled.Sentiment.ConsoleApp/bit/Release/netcoreapp2.0/Wikiled.Sentiment.ConsoleApp.dll test -Input=[path to files] -out=Result -ExtractStyle]

## C# Library 


### Training model

```
var factory = MainContainerFactory.Setup()
                .Config()
                .Splitter();

container = factory.Create().StartSession();
ITrainingClient client = container.GetTraining(Model);
await client.Train(reviews).ConfigureAwait(false);

```

### Testing model

```
client = container.GetTesting(Model);
client.Process(reviews);
```
