using System;
using System.Reactive.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public class ProcessingPipeline
    {
        private ISplitterFactory factory;

        private PerformanceMonitor monitor;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public ProcessingPipeline(ISplitterFactory factory)
        {
            Guard.NotNull(() => factory, factory);
            this.factory = factory;
        }

        public void RegisterStep(IPipelineStep step)
        {
            throw new NotImplementedException();
        }

        public void Start(POSTaggerType value)
        {
            throw new NotImplementedException();
            //monitor = new PerformanceMonitor(100);
            //IObservable<IParsedDocumentHolder> data = null;

            //using (Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(item => log.Info(monitor)))
            //{
            //    var selectedData = data
            //        .SelectMany(item => Observable.Start(() => AdditionalProcessing(item)))
            //        .Merge();
            //    selectedData.LastOrDefaultAsync().Wait();
            //}
        }

        private void ProcessStep(IPipelineStep step)
        {
            
        }
    }
}
