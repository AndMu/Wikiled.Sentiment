using System;
using System.IO;
using Newtonsoft.Json;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Resources
{
    public class JsonStreamingWriter : IDisposable
    {
        private readonly StreamWriter streamWriter;

        private readonly JsonTextWriter writer;

        private int counter;

        public JsonStreamingWriter(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            streamWriter = new StreamWriter(path);
            writer = new JsonTextWriter(streamWriter);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();
        }

        public void WriteObject<T>(T instance)
        {
            counter++;
            var json = JsonConvert.SerializeObject(instance);

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
                streamWriter?.Dispose();
                ((IDisposable)writer)?.Dispose();
            }
        }
    }
}
