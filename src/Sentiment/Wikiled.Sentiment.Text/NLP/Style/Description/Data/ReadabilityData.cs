using Wikiled.Sentiment.Text.NLP.Style.Readability;

namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class ReadabilityData : INLPDataItem
    {
        /// <summary>
        /// Flesch-Kincaid Reading Ease http://en.wikipedia.org/wiki/Flesch%E2%80%93Kincaid_readability_test
        /// </summary>
        public double ReadingEase { get; set; }

        /// <summary>
        /// Flesch-Kincaid Reading Ease http://en.wikipedia.org/wiki/Flesch%E2%80%93Kincaid_readability_test
        /// </summary>
        public FleschReadingEase ReadingEaseCategory
        {
            get
            {
                if (ReadingEase < 0)
                {
                    return FleschReadingEase.Complicated;
                }

                if (ReadingEase <= 30)
                {
                    return FleschReadingEase.UniversityGraduate;
                }

                if (ReadingEase <= 70)
                {
                    return FleschReadingEase.Student13to15Years;
                }

                return ReadingEase <= 100 ? FleschReadingEase.Student11Years : FleschReadingEase.Basic;
            }
        }

        /// <summary>
        /// Flesch–Kincaid Grade Level
        /// http://en.wikipedia.org/wiki/Flesch%E2%80%93Kincaid_readability_test
        /// </summary>
        public double GradeLevel { get; set; }

        /// <summary>
        /// Simplified Gunning Fog Index version
        /// </summary>
        public double GunningFogIndexSimplified { get; set; }

        public double ColemanLiauFormula { get; set; }

        public double AutomatedReadabilityIndex { get; set; }

        public double LixFormula { get; set; }

        public double SMOGIndex { get; set; }

        public object Clone()
        {
            return new ReadabilityData
            {
                AutomatedReadabilityIndex = AutomatedReadabilityIndex,
                ColemanLiauFormula = ColemanLiauFormula,
                GradeLevel = GradeLevel,
                GunningFogIndexSimplified = GunningFogIndexSimplified,
                LixFormula = LixFormula,
                ReadingEase = ReadingEase,
                SMOGIndex = SMOGIndex
            };
        }
    }
}
