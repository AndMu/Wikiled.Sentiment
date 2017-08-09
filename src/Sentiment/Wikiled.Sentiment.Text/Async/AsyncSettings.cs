using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace Wikiled.Sentiment.Text.Async
{
    public class AsyncSettings
    {
        public static ParallelOptions DefaultParallel { get; } = new ParallelOptions
                                                                 {
                                                                     MaxDegreeOfParallelism = Environment.ProcessorCount
                                                                 };

        public static TaskScheduler DefaultScheduler { get; } = new LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount);
    }
}
