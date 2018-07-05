using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
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
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            Splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.reviews = reviews ?? throw new ArgumentNullException(nameof(reviews));
        }

        public ISplitterHelper Splitter { get; }

        public PerformanceMonitor Monitor { get; private set; }

        public SemaphoreSlim ProcessingSemaphore { get; set; }

        public IObservable<ProcessingContext> ProcessStep()
        {
            Monitor = new PerformanceMonitor(100);
            var selectedData = reviews
                .Select(item => Observable.Start(() => StepProcessing(item), scheduler))
                .Merge()
                .Merge();

            return selectedData.Where(item => item != null);
        }

        private async Task<ProcessingContext> StepProcessing(IParsedDocumentHolder reviewHolder)
        {
            try
            {
                Monitor.ManualyCount();
                if (ProcessingSemaphore != null)
                {
                    var isSuccesful = await ProcessingSemaphore.WaitAsync(TimeSpan.FromMinutes(2)).ConfigureAwait(false);
                    if (!isSuccesful)
                    {
                        throw new TimeoutException();
                    }
                }

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
