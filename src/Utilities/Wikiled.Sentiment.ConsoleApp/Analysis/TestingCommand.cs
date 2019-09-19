﻿using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Extensions;
using Wikiled.Common.Utilities.Serialization;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.ConsoleApp.Analysis.Config;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.ConsoleApp.Analysis
{
    /// <summary>
    ///     test [-Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml"] [-Model=.\Svm] -Out=.\results
    /// </summary>
    [Description("pSenti testing")]
    public class TestingCommand : BaseRawCommand<TestingConfig>
    {
        private CsvWriter csvDataOut;

        private JsonStreamingWriter resultsWriter;

        public TestingCommand(ILogger<TestingCommand> log, TestingConfig config, IDataLoader loader, ISessionContainer container)
            : base(log, config, loader, container)
        {
            Semaphore = new SemaphoreSlim(3000);
        }

        protected override async Task Process(IObservable<IParsedDocumentHolder> reviews, ISessionContainer container, ISentimentDataHolder sentimentAdjustment)
        {
            ITestingClient client;
            Config.Out.EnsureDirectoryExistence();
            using (var streamWriter = new StreamWriter(Path.Combine(Config.Out, "results.csv"), false))
            using (csvDataOut = new CsvWriter(streamWriter))
            using (resultsWriter = JsonStreamingWriter.CreateJson(Path.Combine(Config.Out, "result.json")))
            {
                SetupHeader();
                client = container.GetTesting(Config.Model);
                container.Context.Lexicon = sentimentAdjustment;
                var dictionary = container.Resolve<INRCDictionary>();
                using (Observable.Interval(TimeSpan.FromSeconds(30))
                    .Subscribe(item => Logger.LogInformation(client.Pipeline.Monitor.ToString())))
                {
                    client.TrackArff = Config.TrackArff;
                    client.UseBagOfWords = Config.UseBagOfWords;
                    client.Init();
                    await client.Process(reviews.ObserveOn(TaskPoolScheduler.Default))
                        .Select(
                            item =>
                            {
                                SaveDocument(dictionary, item);
                                Semaphore.Release();
                                return item;
                            })
                        .LastOrDefaultAsync();
                }

                if (!Config.TrackArff)
                {
                    client.Save(Config.Out);
                }
            }

            Logger.LogInformation($"Testing performance {client.GetPerformanceDescription()}");
        }

        private void SaveDocument(INRCDictionary dictionary, ProcessingContext context)
        {
            var vector = new SentimentVector();
            if (Config.ExtractStyle)
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
                if (Config.ExtractStyle)
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

            if (Config.Debug)
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
            if (Config.ExtractStyle)
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