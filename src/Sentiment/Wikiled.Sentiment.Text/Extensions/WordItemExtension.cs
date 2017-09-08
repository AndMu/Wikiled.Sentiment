using System;
using System.Collections.Generic;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class WordItemExtension
    {
        public static IEnumerable<string> GetPossibleText(this IWordItem word)
        {
            yield return word.Text;

            if (!string.IsNullOrEmpty(word.Stemmed) &&
                word.Stemmed != word.Text)
            {
                yield return word.Stemmed;
            }

            if (word.Entity == NamedEntities.Hashtag &&
                word.Text.Length > 1)
            {
                yield return word.Text.Substring(1);
            }
        }

        public static bool IsConjunction(this IWordItem word)
        {
            return word.POS.WordType == WordType.Conjunction ||
                   WordTypeResolver.Instance.IsInvertingConjunction(word.Text) ||
                   WordTypeResolver.Instance.IsSpecialEndSymbol(word.Text) ||
                   WordTypeResolver.Instance.IsRegularConjunction(word.Text) ||
                   WordTypeResolver.Instance.IsSubordinateConjunction(word.Text);
        }

        public static bool IsSimpleConjunction(this IWordItem word)
        {
            return word.POS == POSTags.Instance.Comma ||
                   word.POS == POSTags.Instance.Colon ||
                   word.POS == POSTags.Instance.SentenceFinalPunctuation;
        }

        public static bool IsItemBelonging(this IWordItem word)
        {
            return word.Text != word.Stemmed && (word.Text.IsEnding("'s") || word.Text.IsEnding("'"));
        }

        public static bool IsVerbLook(this IWordItem word)
        {
            return word.Text != word.Stemmed && (word.Text.IsEnding("ing") || word.Text.IsEnding("ed"));
        }

        public static bool IsUsedInSentiment(this IWordItem word)
        {
            var inverted = word.GetInverted();
            return word.IsInvertor && inverted != null && inverted.IsSentiment;
        }

        public static Tuple<IWordItem, double> GetInvertedTarget(this IWordItem word)
        {
            bool Forward(IWordItem item) => item.IsTopAttribute || item.IsSentiment || item.IsFeature || item.POS.WordType == WordType.Verb;
            bool Backward(IWordItem item) => item.IsTopAttribute || item.IsSentiment || item.IsFeature;
            return word.Relationship.DistanceToNearest(Forward, Backward);
        }

        public static IWordItem GetInverted(this IWordItem word)
        {
            var distance = word.GetInvertedTarget();
            return distance?.Item1;
        }

        public static bool IsAttached(this IWordItem word)
        {
            return word.IsInvertor && word.GetInverted() != null;
        }

        public static bool CanNotBeAttribute(this IWordItem word)
        {
            if (word.Text.ToLower().Contains("emoticon_"))
            {
                return false;
            }

            return
                word.IsFixed ||
                   word.IsQuestion ||
                   word.IsInvertor ||
                   word.IsStopWord ||
                   word.QuantValue.HasValue ||
                   word.IsItemBelonging() ||
                   !word.Text.HasLetters() ||
                   word.Text.Length < 2 || // feature is at least 3 letters 
                   word.Entity != NamedEntities.None ||
                   word.POS.WordType == WordType.Symbol ||
                   word.IsConjunction() ||
                   word.POS.WordType == WordType.Unknown;
        }

        public static bool CanNotBeFeature(this IWordItem word)
        {
            if (word.Entity == NamedEntities.Location ||
                word.Entity == NamedEntities.Organization ||
                word.Entity == NamedEntities.Person ||
                word.Entity == NamedEntities.Hashtag)
            {
                return false;
            }

            var text = word.Text.ToLower();

            // it was successfully trimmed
            return word.IsVerbLook() ||
                   word.IsConjunction() ||
                   !word.Text.HasLetters() ||

                   // named entities can't be features
                   // named entities except organizations
                   word.POS.WordType == WordType.Symbol ||
                   word.POS.WordType == WordType.Number ||
                   word.IsStopWord ||
                   word.IsInvertor ||
                   word.QuantValue.HasValue ||
                   word.IsQuestion ||
                   word.Entity == NamedEntities.Number ||
                   word.Entity == NamedEntities.Percent ||
                   word.Entity == NamedEntities.Money ||
                   word.Entity == NamedEntities.Date ||
                   word.Entity == NamedEntities.Duration ||
                   word.Entity == NamedEntities.Ordinal ||
                   word.Entity == NamedEntities.Time ||
                   word.Text.Length < 2 || // feature is at least 3 letters 
                   word.IsItemBelonging() ||
                   text.IsEnding("thing") || // things are too generic to be features
                   text.Contains("emoticon_") ||
                   text == "everyone" ||
                   text == "someone" ||
                   text == "anyone" ||
                   text == "one" ||
                   text == "two" ||
                   text == "three" ||
                   text == "four" ||
                   text == "five" ||
                   text == "use" ||
                   text == "about" ||
                   word.POS.Tag == "IN" ||
                   string.Compare(word.Stemmed, "other", StringComparison.OrdinalIgnoreCase) == 0 ||
                   string.Compare(word.Stemmed, "thing", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string GenerateMask(this IWordItem wordItem, bool pure)
        {
            var word = wordItem.Stemmed;
            if (!word.HasLetters())
            {
                return string.Empty;
            }

            if (wordItem.IsFeature)
            {
                if (pure)
                {
                    return word;
                }

                word = "xxxFeature";
            }

            if (wordItem.IsInvertor)
            {
                return !wordItem.IsAttached() ? "xxxNOTxxx" : null;
            }

            if (wordItem.Relationship.Inverted != null)
            {
                return "NOTxxx" + word;
            }

            return wordItem.IsFeature ? null : word;
        }
    }
}
