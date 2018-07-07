using System;
using System.Linq;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Reflection;
using Wikiled.Text.Analysis.Words;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    public class SentenceSurface : IDataSource
    {
        private readonly SentenceSurfaceData sentenceSurface = new SentenceSurfaceData();

        public SentenceSurface(TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        ///     Average sentence length
        /// </summary>
        [InfoField("Average sentence length")]
        public double AverageLength => sentenceSurface.AverageLength;

        /// <summary>
        ///     Percentage of sentences that begin with a subordinating or coordinating conjunctions
        /// </summary>
        [InfoField("Percentage of sentences that begin with a conjunctions")]
        public double PercentOfBeginningWithConjunction => sentenceSurface.PercentOfBeginningWithConjunction;

        /// <summary>
        ///     Percentage of long sentences (sentences greater than 15 words)
        /// </summary>
        [InfoField("Percentage of long sentences")]
        public double PercentOfLong => sentenceSurface.PercentOfLong;

        /// <summary>
        ///     Percentage of sentences that are questions
        /// </summary>
        [InfoField("Percentage of sentences that are questions")]
        public double PercentOfQuestion => sentenceSurface.PercentOfQuestion;

        /// <summary>
        ///     Percentage of short sentences (sentences less than 8 words)
        /// </summary>
        [InfoField("Percentage of short sentences")]
        public double PercentOfShort => sentenceSurface.PercentOfShort;

        public TextBlock Text { get; }

        public SentenceSurfaceData GetData()
        {
            return (SentenceSurfaceData)sentenceSurface.Clone();
        }

        public void Load()
        {
            sentenceSurface.AverageLength = Text.Sentences.Sum(item => item.Text.Length) / (double)Text.Sentences.Length;
            sentenceSurface.PercentOfLong = Text.Sentences.Count(item => item.Words.Count > 15) / (double)Text.Words.Length;
            sentenceSurface.PercentOfShort = Text.Sentences.Count(item => item.Words.Count < 8) /
                                             (double)Text.Words.Length;
            sentenceSurface.PercentOfQuestion = Text.Sentences.Count(item => item.IsQuestion()) /
                                                (double)Text.Words.Length;
            sentenceSurface.PercentOfBeginningWithConjunction = Text.Sentences.Count(
                item => item.Words.Count > 0 &&
                        WordTypeResolver.Instance.IsConjunction(item.Words[0].Text)) / (double)Text.Words.Length;
        }
    }
}
