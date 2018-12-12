using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Words
{
    public class PhraseContructor : IPhraseContructor
    {
        private readonly IWordFactory handler;

        private static readonly ILogger log = ApplicationLogging.CreateLogger<PhraseContructor>();

        public PhraseContructor(IWordFactory handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IEnumerable<IPhrase> GetPhrases(IWordItem word)
        {
            if (word == null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            log.LogDebug("GetPhrases {0}", word);
            IWordItem[] currentWords = word.Relationship.Part.Occurrences
                                   .Where(item => !item.CanNotBeFeature() && !item.IsSentiment).ToArray();

            if (currentWords.Length <= 1)
            {
                yield break;
            }

            var all = string.Join(" ", currentWords.Select(item => item.Text).ToArray());
            var wordIndex = Array.IndexOf(currentWords, word);
            if (wordIndex < 0)
            {
                log.LogDebug("{0} is not found in important list in <{1}>", word, all);
                yield break;
            }

            var nGramBlocks = new List<NGramBlock>();
            var wordsTable = new Dictionary<WordEx, IWordItem>();
            var words = new WordEx[currentWords.Length];
            foreach (IWordItem item in currentWords)
            {
                WordEx wordEx = WordExFactory.Construct(item);
                words[wordsTable.Count] = wordEx;
                wordsTable[wordEx] = item;
            }

            nGramBlocks.AddRange(words.GetNearNGram(wordIndex, 3));
            nGramBlocks.AddRange(words.GetNearNGram(wordIndex, 2));
            foreach (NGramBlock nGramBlock in nGramBlocks)
            {
                IPhrase phrase = handler.CreatePhrase("NP");
                foreach (WordEx occurence in nGramBlock.WordOccurrences)
                {
                    phrase.Add(wordsTable[occurence]);
                }

                yield return phrase;
            }
        }
    }
}
