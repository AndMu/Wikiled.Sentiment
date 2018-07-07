using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wikiled.Sentiment.Text.Resources
{
    public class JsonStreamingWriter : IDisposable
    {
        private readonly StreamWriter streamWriter;

        private readonly JsonTextWriter writer;

        private int counter;

        public JsonStreamingWriter(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            }

            streamWriter = new StreamWriter(path, false, Encoding.UTF8);
            streamWriter.AutoFlush = true;
            writer = new JsonTextWriter(streamWriter);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();
        }

        public void WriteObject<T>(T instance)
        {
            counter++;
            var json = JsonConvert.SerializeObject(instance, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            json = JValue.Parse(json).ToString(Formatting.Indented);
            lock (writer)
            {
                if (counter > 1)
                {
                    writer.WriteRaw($",{Environment.NewLine}");
                }

                writer.WriteRaw(json);
            }
        }

        public void Dispose()
        {
            lock (writer)
            {
                writer.WriteEndArray();
                writer.Close();
                ((IDisposable)writer)?.Dispose();
                streamWriter?.Dispose();
            }
        }
    }
}
