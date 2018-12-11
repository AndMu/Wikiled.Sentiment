using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Aspects.Data;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectSentimentTracker
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<AspectSentimentTracker>();

        private readonly IContextSentimentFactory factory;

        private readonly Dictionary<string, List<SentimentValue>> table = new Dictionary<string, List<SentimentValue>>(StringComparer.OrdinalIgnoreCase);

        private readonly object syncRoot = new object();

        private int totalReviews;

        public AspectSentimentTracker(IContextSentimentFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void Process(IParsedReview review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            log.LogDebug("Process");
            Interlocked.Increment(ref totalReviews);
            foreach (IWordItem aspect in review.Items.Where(item => item.IsFeature))
            {
                IContextSentiment context = factory.Construct(aspect.Relationship);
                lock (syncRoot)
                {
                    if (!table.TryGetWordValue(aspect, out List<SentimentValue> sentiments))
                    {
                        sentiments = new List<SentimentValue>();
                        table[aspect.Stemmed] = sentiments;
                    }

                    sentiments.AddRange(context.Sentiments.Where(item => item.DataValue.Value != 0));
                }
            }
        }

        public AspectSentimentData GetResults()
        {
            log.LogDebug("Save");
            AspectSentimentData data = new AspectSentimentData
            {
                TotalReviews = totalReviews
            };
            lock (syncRoot)
            {
                data.Records = table
                    .OrderByDescending(item => item.Key.Length)
                    .Where(item => item.Value.Count > 0)
                    .Select(
                        item => new AspectSentimentItem
                        {
                            Text = item.Key,
                            Times = item.Value.Count,
                            Sentiment = item.Value.Select(x => x.DataValue).Accumulate().RawRating
                        }).ToArray();
            }

            return data;
        }
    }
}
