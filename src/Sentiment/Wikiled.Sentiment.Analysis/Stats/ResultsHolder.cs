using System;
using System.Collections.Concurrent;
using System.IO;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Analysis.Stats
{
    public class ResultsHolder
    {
        private readonly ConcurrentDictionary<string, Tuple<double, double?>> results = new ConcurrentDictionary<string, Tuple<double, double?>>(StringComparer.OrdinalIgnoreCase);

        public void AddResult(string id, double expected, double? actual)
        {
            Guard.NotNullOrEmpty(() => id, id);
            results.TryAdd(id, new Tuple<double, double?>(expected, actual));
        }

        public void Save(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);
            using (var writer = new StreamWriter(name, false))
            {
                writer.WriteLine("Id,Original,Calculated");
                foreach (var result in results)
                {
                    var actual = result.Value.Item2 == null ? string.Empty : result.Value.Item2.ToString();
                    writer.WriteLine($"{result.Key},{result.Value.Item1},{actual}");
                }
            }
        }
    }
}
