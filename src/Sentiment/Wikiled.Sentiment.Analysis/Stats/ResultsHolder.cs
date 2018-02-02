using System.Collections.Concurrent;
using System.IO;
using CsvHelper;
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
            using (var csvDataOut = new CsvWriter(writer))
            {
                csvDataOut.WriteField("Id");
                csvDataOut.WriteField("Date");
                csvDataOut.WriteField("Original");
                csvDataOut.WriteField("Calculated");
                csvDataOut.WriteField("Total");
                csvDataOut.NextRecord();
                foreach (var result in results)
                {
                    csvDataOut.WriteField(result.Id);
                    csvDataOut.WriteField(result.Date);
                    csvDataOut.WriteField(result.Expected);
                    csvDataOut.WriteField(result.Actual);
                    csvDataOut.WriteField(result.TotalSentiments);
                    csvDataOut.NextRecord();
                }
            }
        }
    }
}
