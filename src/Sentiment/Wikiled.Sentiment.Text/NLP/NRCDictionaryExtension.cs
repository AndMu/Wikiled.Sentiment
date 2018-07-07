using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public static class NRCDictionaryExtension
    {
        public static NRCRecord FindRecord(this INRCDictionary dictionary, WordEx word)
        {
            return word.UnderlyingWord is IWordItem item ? dictionary.FindRecord(item) : dictionary.FindRecord(word.Text);
        }

        public static NRCRecord FindRecord(this INRCDictionary dictionary, IWordItem word)
        {
            NRCRecord nrcRecord = null;
            foreach (var text in word.GetPossibleText())
            {
                nrcRecord = dictionary.FindRecord(text);
                if (nrcRecord != null)
                {
                    break;
                }
            }

            if (nrcRecord == null)
            {
                return null;
            }

            nrcRecord = (NRCRecord)nrcRecord.Clone();
            if (word.Relationship?.Inverted != null)
            {
                nrcRecord.Invert();
            }

            return nrcRecord;
        }

        public static SentimentVector Extract(this INRCDictionary dictionary, IEnumerable<WordEx> words)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (words is null)
            {
                throw new ArgumentNullException(nameof(words));
            }

            var vector = new SentimentVector();
            foreach (var word in words)
            {
                vector.ExtractData(dictionary.FindRecord(word));
            }

            return vector;
        }

        public static SentimentVector Extract(this INRCDictionary dictionary, IEnumerable<IWordItem> words)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (words is null)
            {
                throw new ArgumentNullException(nameof(words));
            }

            var vector = new SentimentVector();
            dictionary.ExtractToVector(vector, words);
            return vector;
        }

        public static void ExtractToVector(this INRCDictionary dictionary, SentimentVector vector, IEnumerable<IWordItem> words)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (vector is null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            if (words is null)
            {
                throw new ArgumentNullException(nameof(words));
            }

            foreach (var word in words)
            {
                vector.ExtractData(dictionary.FindRecord(word));
            }
        }
    }
}
