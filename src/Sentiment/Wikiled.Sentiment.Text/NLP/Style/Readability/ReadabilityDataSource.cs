using System;
using System.Linq;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Reflection;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style.Readability
{
    /// <summary>
    /// Text Readability
    /// </summary>
    public class ReadabilityDataSource : IDataSource
    {
        private readonly ReadabilityData data = new ReadabilityData();

        public ReadabilityDataSource(TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Flesch-Kincaid Reading Ease http://en.wikipedia.org/wiki/Flesch%E2%80%93Kincaid_readability_test
        /// </summary>
        [InfoField("Flesch-Kincaid Reading Ease")]
        public double ReadingEase => data.ReadingEase;

        /// <summary>
        /// Flesch-Kincaid Reading Ease http://en.wikipedia.org/wiki/Flesch%E2%80%93Kincaid_readability_test
        /// </summary>
        public FleschReadingEase ReadingEaseCategory => data.ReadingEaseCategory;

        /// <summary>
        /// Flesch–Kincaid Grade Level
        /// http://en.wikipedia.org/wiki/Flesch%E2%80%93Kincaid_readability_test
        /// </summary>
        [InfoField("Flesch–Kincaid Grade Level")]
        public double GradeLevel => data.GradeLevel;

        /// <summary>
        /// Simplified Gunning Fog Index version
        /// </summary>
        [InfoField("Simplified Gunning Fog Index version")]
        public double GunningFogIndexSimplified => data.GunningFogIndexSimplified;

        [InfoField("Coleman Liau Formula")]
        public double ColemanLiauFormula => data.ColemanLiauFormula;

        [InfoField("Automated Readability Index")]
        public double AutomatedReadabilityIndex => data.AutomatedReadabilityIndex;

        [InfoField("Lix Formula")]
        public double LixFormula => data.LixFormula;

        [InfoField("SMOG Index")]
        public double SMOGIndex => data.SMOGIndex;

        public TextBlock Text { get; }

        public void Load()
        {
            int totalWords = Text.PureWords.Length;
            WordEx[] words = Text.PureWords;
            var wordsSentenceRatio = totalWords / (double)Text.Sentences.Length;
            var charactersWordsRatio = Text.TotalCharacters / (double)totalWords;
            var syllablesWordRatio = Text.Surface.Words.TotalSyllables / (double)totalWords;

            data.ReadingEase = 206.835 - 1.015 * wordsSentenceRatio - 84.6 * syllablesWordRatio;
            data.GradeLevel = (0.39 * wordsSentenceRatio) + (11.8 * syllablesWordRatio) - 15.59;
            data.GunningFogIndexSimplified = 0.4 * ((wordsSentenceRatio +
                                              100 * (double)words.Count(item => item.CountSyllables() > 3) / totalWords));
            data.ColemanLiauFormula = 0.0588 * charactersWordsRatio * 100 -
                                 0.296 * (Text.Sentences.Length / (double)totalWords * (double)100) - 15.8;
            data.AutomatedReadabilityIndex = 4.71 * charactersWordsRatio + 0.5 * wordsSentenceRatio - 21.43;
            data.LixFormula = wordsSentenceRatio + 100 * ((double)words.Count(item => item.Text.Length >= 6) / totalWords);
            data.SMOGIndex = 1.403 * Math.Sqrt((words.Count(item => item.CountSyllables() > 3) * 30 /
                                         (double)Text.Sentences.Length)) + 3.1291;
        }

        public ReadabilityData GetData()
        {
            return (ReadabilityData)data.Clone();
        }
    }
}
