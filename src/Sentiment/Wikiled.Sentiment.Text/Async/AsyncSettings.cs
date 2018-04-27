using System;
using System.Threading.Tasks;

namespace Wikiled.Sentiment.Text.Async
{
    public class AsyncSettings
    {
        public static ParallelOptions DefaultParallel { get; } = new ParallelOptions
                                                                 {
                                                                     MaxDegreeOfParallelism = Environment.ProcessorCount
                                                                 };
    }
}
