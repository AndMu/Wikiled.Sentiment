using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Words
{
    public class PhraseContructor : IPhraseContructor
    {
        private readonly IWordsHandler handler;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public PhraseContructor(IWordsHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IEnumerable<IPhrase> GetPhrases(IWordItem word)
        {
            if (word == null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            log.Debug("GetPhrases {0}", word);
            var currentWords = word.Relationship.Part.Occurrences
                .Where(item => !item.CanNotBeFeature() && !item.IsSentiment).ToArray();

            if (currentWords.Length <= 1)
            {
                yield break;
            }

            var all = string.Join(" ", currentWords.Select(item => item.Text).ToArray());

            int wordIndex =  Array.IndexOf(currentWords, word);
            if (wordIndex < 0)
            {
                log.Debug("{0} is not found in important list in <{1}>", word, all);
                yield break;
            }
            
            List<NGramBlock> nGramBlocks = new List<NGramBlock>();
            nGramBlocks.AddRange(currentWords.GetNearNGram(wordIndex, 3));
            nGramBlocks.AddRange(currentWords.GetNearNGram(wordIndex, 2));
            foreach (var nGramBlock in nGramBlocks)
            {
                var phrase = handler.WordFactory.CreatePhrase("NP");
                foreach (var occurence in nGramBlock.WordOccurrences)
                {
                    phrase.Add(occurence);
                }

                yield return phrase;
            }
        }
    }
}
