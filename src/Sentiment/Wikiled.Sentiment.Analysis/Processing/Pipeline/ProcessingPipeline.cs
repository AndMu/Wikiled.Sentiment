using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IObservable<IParsedDocumentHolder> reviews;

        private readonly IScheduler scheduler;

        private readonly IParsedReviewManagerFactory factory;

        public ProcessingPipeline(IScheduler scheduler, ISplitterHelper splitter, IObservable<IParsedDocumentHolder> reviews, IParsedReviewManagerFactory factory)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => reviews, reviews);
            Guard.NotNull(() => scheduler, scheduler);
            Guard.NotNull(() => factory, factory);
            this.scheduler = scheduler;
            Splitter = splitter;
            this.factory = factory;
            this.reviews = reviews;
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
                var review = factory.Create(Splitter.DataLoader, doc).Create();
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
