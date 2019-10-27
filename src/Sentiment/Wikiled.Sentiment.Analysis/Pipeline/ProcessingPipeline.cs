using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
        private readonly ILogger<ProcessingPipeline> log;

        private readonly IScheduler scheduler;

        private readonly Func<Document, IParsedReviewManager> reviewManager;

        public ProcessingPipeline(ILogger<ProcessingPipeline> log, IScheduler scheduler, Func<Document, IParsedReviewManager> reviewManager)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.reviewManager = reviewManager ?? throw new ArgumentNullException(nameof(reviewManager));
        }

        public PerformanceMonitor Monitor { get; private set; }

        public void ResetMonitor()
        {
            long defaultTotal = 100;
            if (Monitor != null)
            {
                defaultTotal = Monitor.Total;
            }

            Monitor = new PerformanceMonitor(defaultTotal);
        }

        public IObservable<ProcessingContext> Processing(IObservable<IParsedDocumentHolder> reviews)
        {
            if (reviews is null)
            {
                throw new ArgumentNullException(nameof(reviews));
            }

            log.LogInformation("Processing");
            ResetMonitor();
            IObservable<ProcessingContext> selectedData = reviews
                .Select(item => Observable.Start(() => StepProcessing(item), scheduler))
                .Merge()
                .Merge();

            return selectedData.Where(item => item != null);
        }

        private async Task<ProcessingContext> StepProcessing(IParsedDocumentHolder reviewHolder)
        {
            if (reviewHolder == null)
            {
                throw new ArgumentNullException(nameof(reviewHolder));
            }

            Document document;
            Document originalDocument = null;
            IParsedReview review = null;
            try
            {
                Monitor.ManualyCount();
                originalDocument = await reviewHolder.GetOriginal().ConfigureAwait(false);
                document = await reviewHolder.GetParsed().ConfigureAwait(false);
                review = reviewManager(document).Create();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
                document = originalDocument.CloneJson();
                document.Status = Status.Error;
            }

            Monitor.Increment();
            var context = new ProcessingContext(originalDocument, document, review);
            return context;
        }
    }
}
