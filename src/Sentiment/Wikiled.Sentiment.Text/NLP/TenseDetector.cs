using System;
using System.Linq;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.NLP
{
    public static class TenseDetector
    {
        public static TenseType ResolveTense(this ISentence sentence)
        {
            Guard.NotNull(() => sentence, sentence);
            var tenses = sentence.Occurrences.Select(item => item.GetTense()).Where(item => item != null).ToArray();
            if (tenses.Length == 0)
            {
                return TenseType.Unknown;
            }

            int past = 0;
            int future = 0;
            int present = 0;

            foreach (var tense in tenses)
            {
                switch (tense)
                {
                    case TenseType.Past:
                        past++;
                        break;
                    case TenseType.Present:
                        present++;
                        break;
                    case TenseType.Future:
                        future++;
                        break;
                    case TenseType.Unknown:
                        break;
                }
            }

            if (past == 0 &&
                present == 0 &&
                future == 0)
            {
                return TenseType.Unknown;
            }

            if (past > future &&
                past > 0)
            {
                return TenseType.Past;
            }

            if (future > past &&
                future > 0)
            {
                return TenseType.Future;
            }

            return TenseType.Present;
        }

        public static TenseType? GetTense(this IWordItem word)
        {
            Guard.NotNull(() => word, word);
            if (word.POS == POSTags.Instance.VBP ||
                word.POS == POSTags.Instance.VBZ)
            {
                return TenseType.Present;
            }

            if (word.POS == POSTags.Instance.VBD ||
                word.POS == POSTags.Instance.VBN)
            {
                return TenseType.Past;
            }

            if (word.POS == POSTags.Instance.MD)
            {
                if (string.Compare(word.Text, "will", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(word.Text, "shall", StringComparison.OrdinalIgnoreCase) == 0 ||
                    word.Text.IndexOf("'ll", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return TenseType.Future;
                }

                if (word.Text.IndexOf("could", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    word.Text.IndexOf("would", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return TenseType.Past;
                }

                return TenseType.Present;
            }

            return null;
        }
    }
}
