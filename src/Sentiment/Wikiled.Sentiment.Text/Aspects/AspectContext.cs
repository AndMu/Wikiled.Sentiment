using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectContext : IAspectContext
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<AspectContext>();

        private readonly ConcurrentBag<IWordItem> attributes = new ConcurrentBag<IWordItem>();

        private readonly ConcurrentBag<IWordItem> features = new ConcurrentBag<IWordItem>();

        private readonly bool includeSentiment;

        private readonly IWordItem[] words;

        private bool processed;

        public AspectContext(bool includeSentiment, IWordItem[] words)
        {
            log.LogDebug("Construct");
            this.includeSentiment = includeSentiment;
            this.words = words ?? throw new System.ArgumentNullException(nameof(words));
        }

        public IEnumerable<IWordItem> GetAttributes()
        {
            return attributes;
        }

        public IEnumerable<IWordItem> GetFeatures()
        {
            return features;
        }

        public void Process()
        {
            log.LogDebug("Process");
            if (processed)
            {
                log.LogWarning("Allready processed");
                return;
            }

            processed = true;
            Parallel.ForEach(words, AsyncSettings.DefaultParallel, ProcessWord);
        }

        private void ProcessWord(IWordItem wordItem)
        {
            if (wordItem.IsQuestion ||
                wordItem.IsStopWord ||
                wordItem.QuantValue.HasValue)
            {
                return;
            }

            if (wordItem.POS.WordType == WordType.Adjective ||
                wordItem.IsSentiment)
            {
                if (wordItem.CanNotBeAttribute() ||
                    wordItem.IsSentiment && !includeSentiment)
                {
                    // if sentiment excluded
                    log.LogDebug("Can't be attribute: {0}", wordItem);
                    return;
                }

                log.LogDebug("Adding attribute: {0}", wordItem);
                attributes.Add(wordItem);
                return;
            }

            if (wordItem.POS.WordType == WordType.Noun ||
                wordItem.Entity == NamedEntities.Organization)
            {
                // words with ending -ing and -ed can't be features
                if (wordItem.CanNotBeFeature())
                {
                    log.LogDebug("Can't feature: {0}", wordItem);
                    return;
                }

                log.LogDebug("Adding feature: {0}", wordItem);
                features.Add(wordItem);
            }
        }
    }
}
