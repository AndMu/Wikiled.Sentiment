using System;
using NLog;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.Words
{
    public class WordOccurenceFactory : IWordFactory
    {
        private readonly IContextWordsHandler wordsHandlers;

        private readonly IRawTextExtractor extractor;

        private readonly IInquirerManager inquirerManager;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public WordOccurenceFactory(IContextWordsHandler wordsHandlers, IRawTextExtractor extractor, IInquirerManager inquirerManager)
        {
            this.wordsHandlers = wordsHandlers ?? throw new ArgumentNullException(nameof(wordsHandlers));
            this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            this.inquirerManager = inquirerManager ?? throw new ArgumentNullException(nameof(inquirerManager));
        }

        public IPhrase CreatePhrase(string posPhrase)
        {
            BasePOSType postType = POSTags.Instance.UnknownPhrase;
            if (!POSTags.Instance.Contains(posPhrase))
            {
                log.Warn("POS not found <{0}>", posPhrase);
            }
            else
            {
                postType = POSTags.Instance.FindType(posPhrase);
            }

            return Phrase.Create(wordsHandlers, postType);
        }

        public IWordItem CreateWord(string word, string wordType)
        {
            return CreateWord(word, POSTags.Instance.FindType(wordType));
        }

        public IWordItem CreateWord(string word, string lemma, string wordType)
        {
            return WordOccurrence.Create(wordsHandlers, extractor, inquirerManager, word, lemma, POSTags.Instance.FindType(wordType));
        }

        public IWordItem CreateWord(string word, BasePOSType wordPosType)
        {
            return WordOccurrence.Create(wordsHandlers, extractor, inquirerManager, word, null, wordPosType);
        }
    }
}
