using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class ContextSentenceRepairHandler : IContextSentenceRepairHandler
    {
        private readonly ILogger<ContextSentenceRepairHandler> log;

        private readonly (string Word, string Replacement)[] replacements;

        public ContextSentenceRepairHandler(
            ILogger<ContextSentenceRepairHandler> log,
            IContextWordsHandler wordsHandler,
            IWordFactory wordFactory,
            IExtendedWords extendedWords)
        {
            if (wordsHandler == null)
            {
                throw new ArgumentNullException(nameof(wordsHandler));
            }

            if (wordFactory == null)
            {
                throw new ArgumentNullException(nameof(wordFactory));
            }

            this.log = log ?? throw new ArgumentNullException(nameof(log));

            log.LogDebug("Construct");
            replacements = extendedWords.GetReplacements()
                                        .Where(item =>
                                                   !wordsHandler.IsKnown(wordFactory.CreateWord(item.Word, "NN")) ||
                                                   wordsHandler.IsKnown(wordFactory.CreateWord(item.Replacement, "NN")))
                                        .ToArray();
        }

        public string Repair(string original)
        {
            var sentence = original;
            ReplacementOption option = ReplacementOption.IgnoreCase | ReplacementOption.WholeWord;
            foreach ((string Word, string Replacement) replacement in replacements)
            {
                sentence = sentence.ReplaceString(replacement.Word, replacement.Replacement, option);
            }

            if (sentence != original)
            {
                log.LogTrace("Sentence repaired!");
            }

            return sentence;
        }
    }
}
