using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.Frequency;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    /// <summary>
    ///     amazontextex -Source="E:\Data\Amazon\Home_&_Kitchen.txt" -Out="E:\Data\Home_&_Kitchen.txt" -UseLemma=true -RemoveStop=true
    /// </summary>
    public class AmazonTextExtractClean : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentBag<IParsedReview> reviews = new ConcurrentBag<IParsedReview>();

        private Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private BasicWordsHandler handler;

        private SimpleTextSplitter splitter;

        private PerformanceMonitor monitor;

        [Required]
        public string Out { get; set; }

        [Required]
        public string Source { get; set; }

        public bool RemoveStop { get; set; }

        public bool UseLemma { get; set; }

        public override void Execute()
        {
            log.Info("Starting...");
            try
            {
                AmazonParserLogic parser = new AmazonParserLogic();
                handler = new BasicWordsHandler(new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance));
                splitter = new SimpleTextSplitter(handler);
                log.Info("Reading lines...");
                var textItems = parser.Parse(Source).Select(item => item.TextData.Text).ToArray();
                log.Info("Building dictionary from {0} reviews...", textItems.Length);
                monitor = new PerformanceMonitor(textItems.Length);
                ProcessReviews(textItems).Wait();
                int top = 50000;
                log.Info("Filter top {0} from {1}...", top, dictionary.Count);
                dictionary = dictionary.OrderByDescending(item => item.Value)
                    .Take(top)
                    .ToDictionary(item => item.Key, item => item.Value);

                log.Info("Final from {0}...", dictionary.Count);

                log.Info("Saving...");
                monitor = new PerformanceMonitor(reviews.Count);
                using (StreamWriter writer = new StreamWriter(Out))
                {
                    foreach (var review in reviews)
                    {
                        foreach (var sentence in review.Sentences)
                        {
                            foreach (var wordItem in sentence.Occurrences)
                            {
                                var words = GetWord(wordItem);
                                foreach (var word in words)
                                {
                                    if (dictionary.ContainsKey(word))
                                    {
                                        writer.Write(word + " ");
                                    }
                                }
                            }

                            writer.Write(". ");
                        }

                        writer.WriteLine();
                        monitor.Increment();
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private IEnumerable<string> GetWord(IWordItem wordItem)
        {
            if (wordItem.IsStopWord && RemoveStop)
            {
                yield break;
            }

            string word = UseLemma ? wordItem.Stemmed : wordItem.Text;
            var items = CreateText(word).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length > 1)
            {
                foreach (var item in items)
                {
                    var cretedWord = handler.WordFactory.CreateWord(item, wordItem.POS);
                    if (!cretedWord.IsStopWord || !RemoveStop)
                    {
                        yield return UseLemma ? cretedWord.Stemmed : cretedWord.Text;
                    }
                }
            }
            else if (items.Length == 1)
            {
                yield return items[0];
            }
        }

        private async Task ProcessReviews(string[] processingText)
        {
            var loadReview = new TransformBlock<string, IParsedReview>(
                async text =>
                {
                    try
                    {
                        var result = await splitter.Process(new ParseRequest(text)).ConfigureAwait(false);
                        var review = result.GetReview(handler);
                        reviews.Add(review);
                        return review;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    return null;
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount / 2,
                    BoundedCapacity = 100
                });

            var processReview = new TransformBlock<IParsedReview, IParsedReview>(
                review => PreProcessBlock(review),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount / 2,
                    BoundedCapacity = 100
                });

            loadReview.LinkTo(processReview, new DataflowLinkOptions { PropagateCompletion = true });
            var completed = new ActionBlock<IParsedReview>(review => monitor.Increment());
            processReview.LinkTo(completed, new DataflowLinkOptions { PropagateCompletion = true });
            foreach (var id in processingText)
            {
                await loadReview.SendAsync(id);
            }

            loadReview.Complete();
            await Task.WhenAll(loadReview.Completion, processReview.Completion, completed.Completion);
        }

        private IParsedReview PreProcessBlock(IParsedReview review)
        {
            if (review == null)
            {
                return null;
            }

            try
            {
                foreach (var wordItem in review.Items)
                {
                    var words = GetWord(wordItem);
                    foreach (var word in words)
                    {
                        lock (dictionary)
                        {
                            int value;
                            if (!dictionary.TryGetValue(word, out value))
                            {
                                value = 0;
                            }

                            value++;
                            dictionary[word] = value;
                        }
                    }
                }

                return review;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return null;
        }

        public static string CreateText(string word)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == '-' || 
                    char.IsLetterOrDigit(word[i]))
                {
                    builder.Append(word[i]);
                }
            }

            return builder.ToString();
        }
    }
}
