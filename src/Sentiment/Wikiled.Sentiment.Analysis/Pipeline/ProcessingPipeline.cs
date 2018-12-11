using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Pipeline
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<ProcessingPipeline>();

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

            log.LogInformation("ProcessStep");
            Monitor = new PerformanceMonitor(100);
            IObservable<ProcessingContext> selectedData = reviews
                .Select(item => Observable.Start(() => StepProcessing(item), scheduler))
                .Merge()
                .Merge();

            return selectedData.Where(item => item != null);
        }

        private async Task<ProcessingContext> StepProcessing(IParsedDocumentHolder reviewHolder)
        {
            Document document = null;
            IParsedReview review = null;
            try
            {
                Monitor.ManualyCount();
                if (ProcessingSemaphore != null)
                {
                    bool isSuccesful = await ProcessingSemaphore.WaitAsync(TimeSpan.FromMinutes(5)).ConfigureAwait(false);
                    if (!isSuccesful)
                    {
                        throw new TimeoutException();
                    }
                }

                document = await reviewHolder.GetParsed().ConfigureAwait(false);
                review = reviewManager(document).Create();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
                document = reviewHolder.Original.CloneJson();
                document.Status = Status.Error;
            }

            Monitor.Increment();
            ProcessingContext context = new ProcessingContext(reviewHolder.Original, document, review);
            return context;
        }
    }
}
