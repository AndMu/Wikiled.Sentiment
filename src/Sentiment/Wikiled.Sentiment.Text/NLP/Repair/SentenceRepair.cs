﻿using System;
using System.Text.RegularExpressions;
using Wikiled.Common.Arguments;
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
            Guard.NotNullOrEmpty(() => mask, mask);
            Guard.NotNull(() => dictionary, dictionary);

            this.mask = mask ?? throw new ArgumentNullException(nameof(mask));
            this.replaceMask = replaceMask;
            this.dictionary = dictionary;
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

            if (VerifyDictionary != null)
            {
                
                var matches = Regex.Matches(originalSentence, mask, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    bool found = false;
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

            return Regex.Replace(originalSentence, mask, replaceMask, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
