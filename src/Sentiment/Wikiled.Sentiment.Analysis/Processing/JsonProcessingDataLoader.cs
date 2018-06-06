using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class JsonProcessingDataLoader : IProcessingData
    {
        private readonly string path;

        public JsonProcessingDataLoader(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            this.path = path;
            All = Observable.Empty<DataPair>();
        }

        public IObservable<DataPair> All { get; private set; }

        public void Load()
        {
            All = Observable.Create<DataPair>(o =>
            {
                using (StreamReader streamReader = new StreamReader(path))
                using (JsonTextReader reader = new JsonTextReader(streamReader))
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

                        string propertyName = reader.Value.ToString();
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
                            o.OnNext(new DataPair(value, instance));
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
