using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectContext : IAspectContext
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IWordItem[] words;

        private bool processed;

        private readonly ConcurrentBag<IWordItem> attributes = new ConcurrentBag<IWordItem>();

        private readonly ConcurrentBag<IWordItem> features = new ConcurrentBag<IWordItem>();
       
        private readonly bool includeSentiment;

        public AspectContext(bool includeSentiment, IWordItem[] words)
        {
            log.Debug("Construct");
            Guard.NotNull(() => words, words);
            this.includeSentiment = includeSentiment;
            this.words = words;
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
            log.Debug("Process");
            if (processed)
            {
                log.Warn("Already processed");
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

            if (wordItem.POS.WordType == WordType.Adjective)
            {
                if (wordItem.CanNotBeAttribute() ||
                    (wordItem.IsSentiment && !includeSentiment)) // if sentiment excluded
                {
                    log.Debug("Can't be attribute: {0}", wordItem);
                    return;
                }

                log.Debug("Adding attribute: {0}", wordItem);
                attributes.Add(wordItem);
                return;
            }

            if (wordItem.POS.WordType == WordType.Noun || wordItem.Entity == NamedEntities.Organization)
            {
                // words with ending -ing and -ed can't be features
                if (wordItem.CanNotBeFeature())
                {
                    log.Debug("Can't feature: {0}", wordItem);
                    return;
                }

                log.Debug("Adding feature: {0}", wordItem);
                features.Add(wordItem);
            }
        }
    }
}
