using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Words
{
    public class WordOccurenceFactory : IWordFactory
    {
        private readonly IWordsHandler wordsHandlers;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public WordOccurenceFactory(IWordsHandler wordsHandlers)
        {
            this.wordsHandlers = wordsHandlers ?? throw new System.ArgumentNullException(nameof(wordsHandlers));
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
            return WordOccurrence.Create(wordsHandlers, word, lemma, POSTags.Instance.FindType(wordType));
        }

        public IWordItem CreateWord(string word, BasePOSType wordPosType)
        {
            return WordOccurrence.Create(wordsHandlers, word, null, wordPosType);
        }
    }
}
