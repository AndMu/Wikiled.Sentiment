using System;
using System.Threading.Tasks;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class ParallelHelper
    {
        public static ParallelOptions Options { get; set; }

        public static int MaxParallel
        {
            get
            {
                if (Options != null)
                {
                    return Options.MaxDegreeOfParallelism;
                }

                var parallel = Environment.ProcessorCount;
                if (parallel > 30)
                {
                    parallel = 30;
                }

                return parallel;
            }
        }
    }
}
