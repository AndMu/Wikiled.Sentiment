using System;
using System.Text.RegularExpressions;
using Wikiled.Text.Analysis.Dictionary;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class SentenceRepair
    {
        private readonly string lightMask;

        private readonly string replaceMask;

        private readonly IWordsDictionary dictionary;

        private readonly string mask;

        public SentenceRepair(IWordsDictionary dictionary, string lightMask, string mask, string replaceMask)
        {
            if (string.IsNullOrEmpty(mask))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(mask));
            }

            this.mask = mask ;
            this.replaceMask = replaceMask ?? throw new ArgumentNullException(nameof(replaceMask));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.lightMask = lightMask;
        }

        public int[] VerifyDictionary { get; set; }

        public string Repair(string originalSentence)
        {
            if (originalSentence == null)
            {
                throw new ArgumentNullException(nameof(originalSentence));
            }

            if (!string.IsNullOrEmpty(lightMask) &&
                originalSentence.IndexOf(lightMask, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return originalSentence;
            }

            if (VerifyDictionary == null)
            {
                return Regex.Replace(originalSentence, mask, replaceMask, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }

            var matches = Regex.Matches(originalSentence, mask, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                var found = false;
                foreach (var item in VerifyDictionary)
                {
                    if (match.Length >= item + 1)
                    {
                        if (!dictionary.IsKnown(match.Groups[item].Value))
                        {
                            found = false;
                            break;
                        }

                        found = true;
                    }
                }

                if (found)
                {
                    var replaced = Regex.Replace(match.Value, mask, replaceMask, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    originalSentence = originalSentence.Replace(match.Value, replaced);
                }
            }

            return originalSentence;

        }
    }
}
