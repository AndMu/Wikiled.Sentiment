using System;
using System.Linq;
using NLog;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class ContextSentenceRepairHandler : IContextSentenceRepairHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ISentenceRepairHandler repairHandler;

        private readonly (string Word, string Replacement)[] replacements;

        public ContextSentenceRepairHandler(ISentenceRepairHandler repairHandler, IContextWordsHandler wordsHandler, IWordFactory wordFactory, IExtendedWords extendedWords)
        {
            log.Debug("Construct");
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
            foreach (var replacement in replacements)
            {
                sentence = sentence.ReplaceString(replacement.Word, replacement.Replacement, option);
            }

            return sentence;
        }
    }
}
