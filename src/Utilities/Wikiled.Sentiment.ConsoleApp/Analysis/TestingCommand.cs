using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NLog;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP.Style.Description;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    /// test [-Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml"] [-Trained=.\Svm] -Out=.\results
    /// </summary>
    [Description("pSenti testing")]
    public class TestingCommand : BaseRawCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private JsonStreamingWriter resultsWriter;

        public override string Name { get; } = "test";

        /// <summary>
        /// Path to pretrained data. If empty will use as basic lexicon
        /// </summary>
        public string Trained { get; set; }

        public bool ExtractStyle { get;set; }

        [Required]
        public string Out { get; set; }

        protected override void Process(IEnumerable<IParsedDocumentHolder> reviews, ISplitterHelper splitter)
        {
            TestingClient client;
            using (resultsWriter = new JsonStreamingWriter<Document>(Path.Combine(Out, "result.json")))
            {
                var pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, splitter, reviews.ToObservable(TaskPoolScheduler.Default));
                using (Observable.Interval(TimeSpan.FromSeconds(30))
                                 .Subscribe(item => log.Info(pipeline.Monitor)))
                {
                    client = new TestingClient(pipeline, Trained);
                    client.UseBagOfWords = UseBagOfWords;
                    client.Init();
                    client.Process()
                          .Select(item => Observable.Start(() => SaveDocument(splitter.DataLoader, item)))
                          .Merge()
                          .LastOrDefaultAsync()
                          .Wait();
                }

                client.Save(Out);
            }

            log.Info($"Testing performance {client.GetPerformanceDescription()}");
        }

        private Document SaveDocument(IWordsHandler handler, ProcessingContext context)
        {
            if (ExtractStyle)
            {
                var extractor = new StyleExtractor(handler, context.Processed);
                var extracted = extractor.Extract();
                resultsWriter.WriteObject(extracted);
            }
            else
            {
                resultsWriter.WriteObject(context.Processed);
            }
            
            return context.Processed;
        }
    }
}
