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
Wikiled.Sentiment.ConsoleApp.exe test -All=[Folder/File] -Out=[OutPut] 
Wikiled.Sentiment.ConsoleApp.exe test -Positive=[Folder/File] -Negative=[Folder/File] -Out=[OutPut]
```

### To override lexicon
```
Wikiled.Sentiment.ConsoleApp.exe test -All=[Folder/File] -Out=[OutPut] -Weights[WeightsFile] -FullWeightReset
```

### To train with non default lexicon
```
Wikiled.Sentiment.ConsoleApp.exe train -Positive=[Folder/File] -Negative=[Folder/File] [-Weights=c:\out\trumpWeights.csv] [-FullWeightReset] -Model=[Path to Model]
```

### To Test with trained model
```
Wikiled.Sentiment.ConsoleApp.exe test -Out=[OutPut] -All=[Folder/File] -Model=[Path to Trained Model]
```

## Docker service

An application is also available as a standalone docker based REST service, available in [GitHub](https://github.com/AndMu/Wikiled.Sentiment.Service)

It is also possible to use free hosted sentiment analysis service using python or REST Api.
Code sample:

```

reviews = ['I love this hello kitty decal! I like that the bow is pink instead of red. Only bad thing is that after putting it on the window there a few air bubbles, but that most likely my fault. Shipped fast too.',
                  'I bought this for my 3 yr old daughter when I took it out the pack it had a bad odour, cute but very cheap material easy to ripe.  When I tried it on her it was too big, but of course she liked it so I kept it. I dressed her up in it and she looked cute.']

user_name = socket.gethostname()
host = 'sentiment2.wikiled.com'
port=80
with SentimentConnection(host=host, port=port, client_id=user_name) as connection:
    analysis = SentimentAnalysis(connection, domain='market')
    for result in analysis.detect_sentiment_text(amazon_reviews):
        if result['Stars'] is None:
            print('No Sentinent')
        else:
            print(f'Sentinment Stars: {result["Stars"]:1.2f}')
```



## Linux support

[Supported OS](https://github.com/dotnet/core/blob/master/release-notes/3.1/3.1-supported-os.md)

* Install [dotnet core](https://www.microsoft.com/net/download/)
* Retrieve GIT repository source
* dotnet build src/Utilities/Wikiled.Sentiment.ConsoleApp --configuration Release
* dotnet src/Utilities/Wikiled.Sentiment.ConsoleApp/bit/Release/netcoreapp3.1/Wikiled.Sentiment.ConsoleApp.dll test -All=[path to files] -out=Result -ExtractStyle]

## C# Library 


### Training model

```
container = MainContainerFactory
					.Setup(service)
					.SetupLocalCache()
					.Config(item => item.SetConfiguration("resources", Path.Combine(TestContext.CurrentContext.TestDirectory, ConfigurationManager.AppSettings["resources"])))
					.Splitter()
					.Create();
ITrainingClient client = container.GetTraining(Model);
await client.Train(reviews).ConfigureAwait(false);

```

### Testing model

```
client = container.GetTesting(Model);
client.Process(reviews);
```


