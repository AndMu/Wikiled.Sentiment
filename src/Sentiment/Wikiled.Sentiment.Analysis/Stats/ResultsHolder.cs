using System.Collections.Concurrent;
using System.IO;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Analysis.Stats
{
    public class ResultsHolder
    {
        private readonly ConcurrentBag<ResultRecord> results = new ConcurrentBag<ResultRecord>();

        public void AddResult(ResultRecord result)
        {
            Guard.NotNull(() => result, result);
            results.Add(result);
        }

        public void Save(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);
            using (var writer = new StreamWriter(name, false))
            {
                writer.WriteLine("Id,Original,Calculated");
                foreach (var result in results)
                {
                    writer.WriteLine(result);
                }
            }
        }
    }
}
