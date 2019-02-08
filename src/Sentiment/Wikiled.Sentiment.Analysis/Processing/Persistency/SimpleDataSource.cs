using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class SimpleDataSource : IDataSource
    {
        private readonly ILogger<SimpleDataSource> logger;

        private readonly string path;

        private readonly SentimentClass? sentimentClass;

        public SimpleDataSource(ILogger<SimpleDataSource> logger, string path, SentimentClass? sentimentClass)
        {
            this.logger = logger;
            this.path = path;
            this.sentimentClass = sentimentClass;
        }

        public IObservable<DataPair> Load()
        {
            return GetReviews().ToObservable();
        }

        public IEnumerable<DataPair> GetReviews()
        {
            logger.LogInformation("Reading: {0}", path);
            if (string.IsNullOrEmpty(path))
            {
                logger.LogWarning("One of paths is empty");
                return new DataPair[] { };
            }

            return GetReview();
        }


        private IEnumerable<DataPair> GetReview()
        {
            if (File.Exists(path))
            {
                var counter = 0;
                foreach (var line in File.ReadLines(path))
                {
                    counter++;
                    var item = new SingleProcessingData(line.SanitizeXmlString());
                    item.Id = counter.ToString();
                    SetStars(item);
                    yield return new DataPair(sentimentClass, Task.FromResult(item));
                }
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    yield return new DataPair(sentimentClass, Task.Run(() => ReadFile(file)));
                }
            }
        }

        private SingleProcessingData ReadFile(string file)
        {
            var fileInfo = new FileInfo(file);
            var result = new SingleProcessingData(File.ReadAllText(file).SanitizeXmlString());
            result.Id = $"{fileInfo.Directory.Name}_{Path.GetFileNameWithoutExtension(fileInfo.Name)}";
            SetStars(result);
            return result;
        }

        private void SetStars(SingleProcessingData processingData)
        {
            switch (sentimentClass)
            {
                case SentimentClass.Positive:
                    SetStars(processingData, 5);
                    break;
                case SentimentClass.Negative:
                    SetStars(processingData, 1);
                    break;
                case SentimentClass.Neutral:
                    SetStars(processingData, 3);
                    break;
            }
        }

        private static void SetStars(SingleProcessingData processingData, double defaultStars)
        {
            if (processingData.Stars == null)
            {
                processingData.Stars = defaultStars;
            }
        }
    }
}