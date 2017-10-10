using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Extraction
{
    [Description("Extract Aspects and features from unlaballed dataset")]
    public class ExtractAttributesCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private PerformanceMonitor monitor;

        private ISplitterHelper splitter;

        public override void Execute()
        {
            //log.Info("Starting...");
            //splitter = new SplitterFactory(new LocalCacheFactory(), new ConfigurationHandler()).Create(POSTaggerType.SharpNLP);

            //monitor = new PerformanceMonitor(100);
            //IObservable<IParsedDocumentHolder> data = null;

            //using (Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(item => log.Info(monitor)))
            //{
            //    var selectedData = data
            //        .SelectMany(item => Observable.Start(() => AdditionalProcessing(item)))
            //        .Merge();
            //    selectedData.LastOrDefaultAsync().Wait();
            //}

            throw new System.NotImplementedException();
        }

        public override string Name { get; } = "extract";


        private async Task<IParsedReview> AdditionalProcessing(IParsedDocumentHolder reviewHolder)
        {
            throw new System.NotImplementedException();
            //try
            //{
            //    monitor.ManualyCount();
            //    var doc = await reviewHolder.GetParsed().ConfigureAwait(false);
            //    var review = doc.GetReview(splitter.DataLoader);
            //    if (review != null)
            //    {
            //        featureExtractor.Process(review);
            //    }

            //    return review;
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex);
            //}
            //finally
            //{
            //    monitor.Increment();
            //}

            //return null;
        }
    }
}
