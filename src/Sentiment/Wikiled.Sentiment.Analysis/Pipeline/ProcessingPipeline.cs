using NLog;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Pipeline
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IScheduler scheduler;

        private readonly Func<Document, IParsedReviewManager> reviewManager;

        public ProcessingPipeline(IScheduler scheduler, Func<Document, IParsedReviewManager> reviewManager)
        {
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.reviewManager = reviewManager ?? throw new ArgumentNullException(nameof(reviewManager));
        }

        public PerformanceMonitor Monitor { get; private set; }

        public SemaphoreSlim ProcessingSemaphore { get; set; }

        public IObservable<ProcessingContext> ProcessStep(IObservable<IParsedDocumentHolder> reviews)
        {
            if (reviews is null)
            {
                throw new ArgumentNullException(nameof(reviews));
            }

            log.Info("ProcessStep");
            Monitor = new PerformanceMonitor(100);
            IObservable<ProcessingContext> selectedData = reviews
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
                    bool isSuccesful = await ProcessingSemaphore.WaitAsync(TimeSpan.FromMinutes(2)).ConfigureAwait(false);
                    if (!isSuccesful)
                    {
                        throw new TimeoutException();
                    }
                }

                Document doc = await reviewHolder.GetParsed().ConfigureAwait(false);
                var review = reviewManager(doc).Create();
                ProcessingContext context = new ProcessingContext(reviewHolder.Original, doc, review);
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
