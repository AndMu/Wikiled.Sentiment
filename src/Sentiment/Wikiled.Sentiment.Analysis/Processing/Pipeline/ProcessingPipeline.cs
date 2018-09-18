using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IScheduler scheduler;

        public ProcessingPipeline(IScheduler scheduler, IContainerHelper container)
        {
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            ContainerHolder = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IContainerHelper ContainerHolder { get; }

        public PerformanceMonitor Monitor { get; private set; }

        public ISentimentDataHolder LexiconAdjustment { get; set; }

        public SemaphoreSlim ProcessingSemaphore { get; set; }

        public IObservable<ProcessingContext> ProcessStep(IObservable<IParsedDocumentHolder> reviews)
        {
            if (reviews is null)
            {
                throw new ArgumentNullException(nameof(reviews));
            }

            log.Info("ProcessStep");
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
                IParsedReview review;
                if (LexiconAdjustment != null)
                {
                    log.Debug("Using lexicon adjustment");
                    review = ContainerHolder.Resolve(doc, LexiconAdjustment).Create();
                }
                else
                {
                    review = ContainerHolder.Resolve(doc).Create();
                }

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
