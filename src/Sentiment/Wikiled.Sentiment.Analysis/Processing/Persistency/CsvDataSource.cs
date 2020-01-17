using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class CsvDataSource : IDataSource
    {
        private readonly ILogger<CsvDataSource> logger;

        private readonly string path;

        public CsvDataSource(ILogger<CsvDataSource> logger, string path)
        {
            this.logger = logger;
            this.path = path;
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
            logger.LogInformation("Loading {0}", path);
            var counter = 0;
            using (var streamRead = new StreamReader(path))
            using (var csvData = new CsvReader(streamRead, CultureInfo.InvariantCulture))
            {
                csvData.Read();
                csvData.ReadHeader();
                csvData.Configuration.MissingFieldFound = null;
                var headerTable = csvData.Context.HeaderRecord.ToLookup(item => item, StringComparer.OrdinalIgnoreCase);
                while (csvData.Read())
                {
                    counter++;
                    var id = counter.ToString();
                    double? stars = null;
                    SentimentClass? sentimentClass = null;
                    if (headerTable.Contains("id"))
                    {
                        id = csvData.GetField(headerTable["id"].First());
                    }

                    if (headerTable.Contains("sentiment"))
                    {
                        sentimentClass = csvData.GetField<SentimentClass?>(headerTable["sentiment"].First());
                    }

                    string author = null;
                    if (headerTable.Contains("userid"))
                    {
                        author = csvData.GetField(headerTable["userid"].First());
                    }

                    if (headerTable.Contains("author"))
                    {
                        author = csvData.GetField(headerTable["author"].First());
                    }

                    if (headerTable.Contains("stars"))
                    {
                        stars = csvData.GetField<double?>(headerTable["stars"].First());
                    }

                    if (headerTable.Contains("text"))
                    {
                        var text = csvData.GetField(headerTable["text"].First());
                        var item = new SingleProcessingData(text.SanitizeXmlString());
                        item.Id = id;
                        item.Author = author;
                        item.Stars = stars;

                        if (stars != null)
                        {
                            sentimentClass = stars > 3 ? SentimentClass.Positive : SentimentClass.Negative;
                        }

                        yield return new DataPair(sentimentClass, Task.FromResult(item));
                    }
                }
            }
        }
    }
}