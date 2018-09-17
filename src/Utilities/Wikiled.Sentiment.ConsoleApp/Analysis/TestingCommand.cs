using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Autofac;
using CsvHelper;
using NLog;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    /// test [-Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml"] [-Model=.\Svm] -Out=.\results
    /// </summary>
    [Description("pSenti testing")]
    public class TestingCommand : BaseRawCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private JsonStreamingWriter resultsWriter;

        private CsvWriter csvDataOut;

        public override string Name { get; } = "test";

        /// <summary>
        /// Path to pretrained data. If empty will use as basic lexicon
        /// </summary>
        public string Model { get; set; }

        public bool Debug { get; set; }

        /// <summary>
        /// Track Arff
        /// </summary>
        public bool TrackArff { get; set; }

        public bool ExtractStyle { get; set; }

        [Required]
        public string Out { get; set; }

        protected override void Process(IObservable<IParsedDocumentHolder> reviews, IContainerHelper container, ISentimentDataHolder sentimentAdjustment)
        {
            TestingClient client;
            Out.EnsureDirectoryExistence();
            using (var streamWriter = new StreamWriter(Path.Combine(Out, "results.csv"), false))
            using (csvDataOut = new CsvWriter(streamWriter))
            using (resultsWriter = new JsonStreamingWriter(Path.Combine(Out, "result.json")))
            {
                SetupHeader();
                var pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, container);
                pipeline.LexiconAdjustment = sentimentAdjustment;
                var dictionary = container.Container.Resolve<INRCDictionary>();
                using (Observable.Interval(TimeSpan.FromSeconds(30))
                                 .Subscribe(item => log.Info(pipeline.Monitor)))
                {
                    client = new TestingClient(pipeline, Model);
                    Semaphore = new SemaphoreSlim(2000);
                    pipeline.ProcessingSemaphore = Semaphore;
                    client.TrackArff = TrackArff;
                    client.UseBagOfWords = UseBagOfWords;
                    client.Init();
                    client.Process(reviews.ObserveOn(TaskPoolScheduler.Default))
                          .Select(
                              item => 
                              {
                                  SaveDocument(dictionary, item);
                                  pipeline.Monitor.Increment();
                                  return item;
                              })
                          .LastOrDefaultAsync()
                          .Wait();
                }

                if (!TrackArff)
                {
                    client.Save(Out);
                }
            }

            log.Info($"Testing performance {client.GetPerformanceDescription()}");
        }

        private void SaveDocument(INRCDictionary dictionary, ProcessingContext context)
        {
            SentimentVector vector = new SentimentVector();
            if (ExtractStyle)
            {
                foreach (var word in context.Processed.Words)
                {
                    vector.ExtractData(dictionary.FindRecord(word));
                }
            }

            lock (csvDataOut)
            {
                csvDataOut.WriteField(context.Original.Id);
                csvDataOut.WriteField(context.Original.DocumentTime);
                csvDataOut.WriteField(context.Original.Stars);
                csvDataOut.WriteField(context.Adjustment.Rating.StarsRating);
                csvDataOut.WriteField(context.Review.GetAllSentiments().Length);
                if (ExtractStyle)
                {
                    csvDataOut.WriteField(vector.Anger);
                    csvDataOut.WriteField(vector.Anticipation);
                    csvDataOut.WriteField(vector.Disgust);
                    csvDataOut.WriteField(vector.Fear);
                    csvDataOut.WriteField(vector.Joy);
                    csvDataOut.WriteField(vector.Sadness);
                    csvDataOut.WriteField(vector.Surprise);
                    csvDataOut.WriteField(vector.Trust);
                    csvDataOut.WriteField(vector.Total);
                }

                csvDataOut.NextRecord();
                csvDataOut.Flush();
            }

            if (Debug)
            {
                resultsWriter.WriteObject(context.Processed);
            }
        }

        private void SetupHeader()
        {
            csvDataOut.WriteField("Id");
            csvDataOut.WriteField("Date");
            csvDataOut.WriteField("Original");
            csvDataOut.WriteField("Calculated");
            csvDataOut.WriteField("TotalSentimentWords");
            if (ExtractStyle)
            {
                csvDataOut.WriteField("Anger");
                csvDataOut.WriteField("Anticipation");
                csvDataOut.WriteField("Disgust");
                csvDataOut.WriteField("Fear");
                csvDataOut.WriteField("Joy");
                csvDataOut.WriteField("Sadness");
                csvDataOut.WriteField("Surprise");
                csvDataOut.WriteField("Trust");
                csvDataOut.WriteField("TotalWords");
            }

            csvDataOut.NextRecord();
        }
    }
}
