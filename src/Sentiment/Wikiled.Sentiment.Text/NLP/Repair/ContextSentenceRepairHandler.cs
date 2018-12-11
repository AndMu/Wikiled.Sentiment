using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class ContextSentenceRepairHandler : IContextSentenceRepairHandler
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<ContextSentenceRepairHandler>();

        private readonly ISentenceRepairHandler repairHandler;

        private readonly (string Word, string Replacement)[] replacements;

        public ContextSentenceRepairHandler(ISentenceRepairHandler repairHandler, IContextWordsHandler wordsHandler, IWordFactory wordFactory, IExtendedWords extendedWords)
        {
            log.LogDebug("Construct");
            if (wordsHandler == null)
            {
                throw new ArgumentNullException(nameof(wordsHandler));
            }

            if (wordFactory == null)
            {
                throw new ArgumentNullException(nameof(wordFactory));
            }

            this.repairHandler = repairHandler ?? throw new ArgumentNullException(nameof(repairHandler));
            replacements = extendedWords.GetReplacements()
                .Where(item =>
                           !wordsHandler.IsKnown(wordFactory.CreateWord(item.Word, "NN")) ||
                           wordsHandler.IsKnown(wordFactory.CreateWord(item.Replacement, "NN")))
                .ToArray();
        }

        public string Repair(string sentence)
        {
            sentence = repairHandler.Repair(sentence);
            ReplacementOption option = ReplacementOption.IgnoreCase | ReplacementOption.WholeWord;
            foreach ((string Word, string Replacement) replacement in replacements)
            {
                sentence = sentence.ReplaceString(replacement.Word, replacement.Replacement, option);
            }

            return sentence;
        }
    }
}
