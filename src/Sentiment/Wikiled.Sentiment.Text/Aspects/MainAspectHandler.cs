using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class MainAspectHandler : IMainAspectHandler
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<MainAspectHandler>();

        private readonly IAspectContextFactory aspectContextFactory;

        private readonly ConcurrentDictionary<string, WordOccurenceTracker> aspects =
            new ConcurrentDictionary<string, WordOccurenceTracker>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, WordOccurenceTracker> attributes =
            new ConcurrentDictionary<string, WordOccurenceTracker>(StringComparer.OrdinalIgnoreCase);

        private readonly int cutOff;

        public MainAspectHandler(IAspectContextFactory aspectContextFactory, int cutOff = 4)
        {
            this.aspectContextFactory = aspectContextFactory ?? throw new ArgumentNullException(nameof(aspectContextFactory));
            this.cutOff = cutOff;
        }

        public IEnumerable<IWordItem> GetAttributes(int total)
        {
            log.LogDebug("GetAttributes: {0}", total);
            return GetWords(attributes, total);
        }

        public IEnumerable<IWordItem> GetFeatures(int total)
        {
            log.LogDebug("GetFeatures: {0}", total);
            return GetWords(aspects, total);
        }

        public void Process(IParsedReview review)
        {
            if (review is null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            foreach (var sentence in review.Sentences)
            {
                var context = aspectContextFactory.Create(sentence.Occurrences.ToArray());
                context.Process();
                ProcessFeatures(aspects, context.GetFeatures());
                ProcessFeatures(attributes, context.GetAttributes());
            }
        }

        private static WordOccurenceTracker GetTracker(ConcurrentDictionary<string, WordOccurenceTracker> table, IWordItem wordItem)
        {
            if (table.TryGetWordValue(wordItem, out var tracker))
            {
                return tracker;
            }

            tracker = new WordOccurenceTracker();
            if (!table.TryAdd(wordItem.Text, tracker))
            {
                table.TryGetWordValue(wordItem, out tracker);
            }

            return tracker;
        }

        private IEnumerable<IWordItem> GetWords(IDictionary<string, WordOccurenceTracker> table, int total)
        {
            var occurences =
                table.Where(item => item.Value.Total >= cutOff)
                    .OrderByDescending(item => item.Value.Total)
                    .Take(total)
                    .ToArray();
            var words = occurences.SelectMany(item => item.Value.Words).ToArray();
            var tableOfWords = new Dictionary<string, IWordItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var wordItem in words)
            {
                if (!tableOfWords.TryGetValue(wordItem.Stemmed, out var previous) ||
                    (previous.Text.Length > wordItem.Text.Length))
                {
                    tableOfWords[wordItem.Stemmed] = wordItem;
                }
            }

            log.LogDebug("GetWords - occurences:{0} words:{1} tableOfWords:{2}", occurences.Length, words.Length, tableOfWords.Count);
            var phrases = occurences.SelectMany(item => item.Value.GetPhrases(cutOff));
            return tableOfWords.Values.Union(phrases);
        }

        private void ProcessFeatures(ConcurrentDictionary<string, WordOccurenceTracker> table, IEnumerable<IWordItem> features)
        {
            foreach (var wordItem in features)
            {
                var tracker = GetTracker(table, wordItem);
                tracker.AddWord(wordItem);
            }
        }
    }
}