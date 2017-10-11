using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private PerformanceMonitor monitor;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IObservable<IParsedDocumentHolder> reviews;

        public ProcessingPipeline(ISplitterHelper splitter, IObservable<IParsedDocumentHolder> reviews)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => reviews, reviews);
            Splitter = splitter;
            this.reviews = reviews.Replay();
        }

        public ISplitterHelper Splitter { get; }

        public IObservable<ProcessingContext> ProcessStep()
        {
            monitor = new PerformanceMonitor(100);
            var selectedData = reviews
                .SelectMany(item => Observable.Start(() => StepProcessing(item)))
                .Merge();

            Observable.Interval(TimeSpan.FromSeconds(30))
                      .TakeUntil(selectedData)
                      .Select(
                          item =>
                          {
                              log.Info(monitor);
                              return item;
                          });

            return selectedData;
        }

        private async Task<ProcessingContext> StepProcessing(IParsedDocumentHolder reviewHolder)
        {
            try
            {
                monitor.ManualyCount();
                var doc = await reviewHolder.GetParsed().ConfigureAwait(false);
                var review = doc.GetReview(Splitter.DataLoader);
                var context = new ProcessingContext(reviewHolder.Original, doc, review);
                return context;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                monitor.Increment();
            }

            return null;
        }
    }
}
