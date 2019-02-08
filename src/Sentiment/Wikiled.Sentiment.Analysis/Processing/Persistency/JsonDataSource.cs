using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Persistency
{
    public class JsonDataSource : IDataSource
    {
        private readonly ILogger<JsonDataSource> logger;

        private readonly string path;

        public JsonDataSource(ILogger<JsonDataSource> logger, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            this.path = path;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IObservable<DataPair> Load()
        {
            return Observable.Create<DataPair>(o =>
            {
                using (var streamReader = new StreamReader(path))
                using (var reader = new JsonTextReader(streamReader))
                {
                    reader.SupportMultipleContent = true;

                    var serializer = new JsonSerializer();
                    reader.Read();
                    while (reader.TokenType != JsonToken.EndObject)
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            reader.Read();
                        }

                        if (reader.TokenType != JsonToken.PropertyName)
                        {
                            throw new FormatException("Expected a property name, got: " + reader.TokenType);
                        }

                        var propertyName = reader.Value.ToString();
                        if (!Enum.TryParse(propertyName, true, out SentimentClass value))
                        {
                            throw new FormatException("Expected Sentiment type but got:" + value);
                        }

                        reader.Read();
                        if (reader.TokenType != JsonToken.StartArray)
                        {
                            throw new FormatException("Expected a start of array, got: " + reader.TokenType);
                        }

                        reader.Read();
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            var instance = serializer.Deserialize<SingleProcessingData>(reader);
                            o.OnNext(new DataPair(value, Task.FromResult(instance)));
                            reader.Read();
                        }

                        reader.Read();
                    }

                    o.OnCompleted();
                    return Disposable.Empty;
                }
            });
        }
    }
}
