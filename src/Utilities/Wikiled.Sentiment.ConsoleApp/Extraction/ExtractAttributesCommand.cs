using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Extraction
{
    [Description("Extract Aspects and features from unlaballed dataset")]
    public class ExtractAttributesCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private MainAspectHandler featureExtractor;

        private ISplitterHelper splitter;

        public override string Name { get; } = "extract";

        [Description("Source of documents")]
        [Required]
        public string Source { get; set; }

        [Description("Place to save output")]
        [Required]
        public string Out { get; set; }

        [Description("Include sentiment words into attributes")]
        public bool Sentiment { get; set; }

        protected override async Task Execute(CancellationToken token)
        {
            log.Info("Starting...");
            featureExtractor = new MainAspectHandler(new AspectContextFactory(Sentiment));
            splitter = new MainSplitterFactory(new LocalCacheFactory(new MemoryCache(new MemoryCacheOptions())), new ConfigurationHandler()).Create(POSTaggerType.SharpNLP);
            var pipeline = new ProcessingPipeline(TaskPoolScheduler.Default, splitter, GetReviews().ToObservable(TaskPoolScheduler.Default), new ParsedReviewManagerFactory());
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                await pipeline.ProcessStep()
                        .Select(item => Observable.Start(() =>
                        {
                            Processing(item);
                            pipeline.Monitor.Increment();
                        }))
                        .Merge()
                        .LastOrDefaultAsync();
            }

            var file = Path.Combine(Out, "features.xml");
            log.Info("Saving {0}...", file);
            AspectSerializer serializer = new AspectSerializer(splitter.DataLoader);
            serializer.Serialize(featureExtractor).Save(file);
        }

        private IEnumerable<IParsedDocumentHolder> GetReviews()
        {
            log.Info("Input {0}", Source);
            return splitter.Splitter.GetParsedReviewHolders(Source, null);
        }

        private void Processing(ProcessingContext reviewHolder)
        {
            featureExtractor.Process(reviewHolder.Review);
        }
    }
}
