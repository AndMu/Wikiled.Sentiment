using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class DataLoader : IDataLoader
    {
        private readonly ILogger<DataLoader> logger;

        private readonly ILoggerFactory loggerFactory;

        public DataLoader(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            logger = this.loggerFactory.CreateLogger<DataLoader>();
        }

        public IDataSource Load(IDataSourceConfig source)
        {
            if (source == null ||
                (source.All == null && source.Negative == null && source.Positive == null))
            {
                throw new ArgumentNullException("Data source was not specified");
            }

            if (source.All != null)
            {
                if (File.Exists(source.All))
                {
                    logger.LogInformation("Loading {0}", source.All);
                    if (source.All.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        return new XmlDataLoader(loggerFactory.CreateLogger<XmlDataLoader>()).LoadOldXml(source.All);
                    }

                    logger.LogInformation("Loading {0} as JSON", source.All);
                    var data = new JsonDataSource(loggerFactory.CreateLogger<JsonDataSource>(), source.All);
                    return data;
                }

                return new SimpleDataSource(loggerFactory.CreateLogger<SimpleDataSource>(), source.All, null);
            }

            var loaders = new List<IDataSource>();
            if (!string.IsNullOrEmpty(source.Negative))
            {
                loaders.Add(new SimpleDataSource(loggerFactory.CreateLogger<SimpleDataSource>(), source.Negative, SentimentClass.Negative));
            }

            if (!string.IsNullOrEmpty(source.Positive))
            {
                loaders.Add(new SimpleDataSource(loggerFactory.CreateLogger<SimpleDataSource>(), source.Negative, SentimentClass.Positive));
            }

            return new CombinedDataSource(loaders.ToArray());
        }
    }

}
