using System;
using System.Reactive.Concurrency;
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
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IObservable<IParsedDocumentHolder> reviews;

        private readonly IScheduler scheduler;

        public ProcessingPipeline(IScheduler scheduler, ISplitterHelper splitter, IObservable<IParsedDocumentHolder> reviews)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => reviews, reviews);
            Guard.NotNull(() => scheduler, scheduler);
            this.scheduler = scheduler;
            Splitter = splitter;
            var replay = reviews.Replay();
            this.reviews = replay;
            replay.Connect();
        }

        public ISplitterHelper Splitter { get; }

        public PerformanceMonitor Monitor { get; private set; }

        public IObservable<ProcessingContext> ProcessStep()
        {
            Monitor = new PerformanceMonitor(100);
            var selectedData = reviews
                .SelectMany(item => Observable.Start(() => StepProcessing(item), scheduler))
                .Merge();

            return selectedData.Where(item => item != null);
        }

        private async Task<ProcessingContext> StepProcessing(IParsedDocumentHolder reviewHolder)
        {
            try
            {
                Monitor.ManualyCount();
                var doc = await reviewHolder.GetParsed().ConfigureAwait(false);
                var review = doc.GetReview(Splitter.DataLoader);
                var context = new ProcessingContext(reviewHolder.Original, doc, review);
                return context;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            Monitor.Increment();
            return null;
        }
    }
}
