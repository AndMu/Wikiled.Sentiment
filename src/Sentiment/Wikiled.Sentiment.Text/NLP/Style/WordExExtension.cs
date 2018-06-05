using Wikiled.Common.Arguments;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Words;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    public static class WordExExtension
    {
        public static bool IsCoordinatingConjunction(this WordEx word)
        {
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return wordItem.POS == POSTags.Instance.CC;
            }

            return WordTypeResolver.Instance.IsCoordinatingConjunctions(word.Text);
        }

        public static bool IsPronoun(this WordEx word)
        {
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return wordItem.POS.WordType == WordType.Pronoun;
            }

            return WordTypeResolver.Instance.IsPronoun(word.Text);
        }

        public static int CountSyllables(this WordEx word)
        {
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return EnglishSyllableCounter.Instance.CountSyllables(wordItem.Text);
            }

            return EnglishSyllableCounter.Instance.CountSyllables(word.Text);
        }

        public static bool IsWordType(this IPOSTagger tagger, WordEx word, WordType type)
        {
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return wordItem.POS.WordType == type;
            }

            return tagger.GetTag(word.Text).WordType == type;
        }

        public static bool IsDigit(this WordEx word)
        {
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return wordItem.Entity == NamedEntities.Number ||
                       wordItem.Entity == NamedEntities.Percent ||
                       wordItem.Entity == NamedEntities.Money ||
                       wordItem.Entity == NamedEntities.Ordinal ||
                       wordItem.POS == POSTags.Instance.CD;
            }

            return false;
        }

        public static bool IsWordType(this IPOSTagger tagger, WordEx word, BasePOSType posType)
        {
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return wordItem.POS == posType;
            }

            return tagger.GetTag(word.Text) == posType;
        }

        public static bool IsQuestion(this WordEx word, IWordsHandler wordsHandler)
        {
            Guard.NotNull(() => wordsHandler, wordsHandler);
            IWordItem wordItem = word.UnderlyingWord as IWordItem;
            if (wordItem != null)
            {
                return wordItem.IsQuestion;
            }

            return wordsHandler.IsQuestion(wordsHandler.WordFactory.CreateWord(word.Text, "NN"));
        }
    }
}
