using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wikiled.Sentiment.Text.Async
{
    public static class ForEachHelper
    {
        public static void ForEachExecute<T>(this IEnumerable<T> collection, Action<T> action)
        {
            Exception exception = null;
            Parallel.ForEach(
                collection,
                AsyncSettings.DefaultParallel,
                (item, loopState) =>
                {
                    try
                    {
                        action(item);
                    }
                    catch(Exception ex)
                    {
                        exception = ex;
                        loopState.Stop();
                    }
                });

            if(exception != null)
            {
                throw exception;
            }
        }
    }
}
