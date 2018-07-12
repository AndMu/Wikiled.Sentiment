using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.Structure;

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
            Dictionary<WordEx, IWordItem> wordsTable = new Dictionary<WordEx, IWordItem>();
            WordEx[] words = new WordEx[currentWords.Length];
            foreach (var item in currentWords)
            {
                var wordEx = WordExFactory.Construct(item);
                words[wordsTable.Count] = wordEx;
                wordsTable[wordEx] = item;
            }

            nGramBlocks.AddRange(words.GetNearNGram(wordIndex, 3));
            nGramBlocks.AddRange(words.GetNearNGram(wordIndex, 2));
            foreach (var nGramBlock in nGramBlocks)
            {
                var phrase = handler.WordFactory.CreatePhrase("NP");
                foreach (var occurence in nGramBlock.WordOccurrences)
                {
                    phrase.Add(wordsTable[occurence]);
                }

                yield return phrase;
            }
        }
    }
}
