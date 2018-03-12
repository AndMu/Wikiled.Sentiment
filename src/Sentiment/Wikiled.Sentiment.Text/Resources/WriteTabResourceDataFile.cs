using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Common.Arguments;

namespace Wikiled.Sentiment.Text.Resources
{
    public class WriteTabResourceDataFile : IDisposable
    {
        private readonly string resourcesPath;

        private readonly StreamWriter writer;

        public WriteTabResourceDataFile(string file)
        {
            Guard.NotNullOrEmpty(() => file, file);
            resourcesPath = Path.Combine(resourcesPath, file);
            writer = new StreamWriter(resourcesPath, false);
        }

        public WriteTabResourceDataFile(StreamWriter stream)
        {
            Guard.NotNull(() => stream, stream);
            writer = stream;
        }

        public bool UseDefaultIfNotFound { get; set; }

        public void Dispose()
        {
            writer?.Dispose();
        }

        public void WriteData<T1, T2>(Dictionary<T1, T2> table)
        {
            Guard.NotNull(() => table, table);
            foreach (var item in table.OrderBy(item => item.Key))
            {
                if (UseDefaultIfNotFound &&
                    EqualityComparer<T2>.Default.Equals(item.Value, default(T2)))
                {
                    writer.WriteLine(item.Key);
                }
                else
                {
                    writer.WriteLine("{0}\t{1}", item.Key, item.Value);
                }
            }
        }
    }
}
