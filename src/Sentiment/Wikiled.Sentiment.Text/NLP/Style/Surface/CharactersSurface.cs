using System;
using System.Linq;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    public class CharactersSurface : IDataSource
    {
        private readonly CharactersSurfaceData data = new CharactersSurfaceData();

        public CharactersSurface(TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        ///     Percentage of all characters that are commas
        /// </summary>
        [InfoField("Percentage of commas")]
        public double PercentOfCommas => data.PercentOfCommas;

        /// <summary>
        ///     Percentage of all characters that are punctuation characters
        /// </summary>
        [InfoField("Percentage of punctuation")]
        public double PercentOfPunctuation => data.PercentOfPunctuation;

        /// <summary>
        ///     Percentage of all characters that are semicolons
        /// </summary>
        [InfoField("Percentage of semicolons")]
        public double PercentOfSemicolons => data.PercentOfSemicolons;

        public TextBlock Text { get; }

        private int TotalCommas
        {
            get
            {
                return Text.Sentences.Sum(item => item.CountCommas());
            }
        }

        private int TotalPunctuation
        {
            get
            {
                return Text.Sentences.Sum(item => item.CountPunctuations());
            }
        }

        private int TotalSemicolons
        {
            get
            {
                return Text.Sentences.Sum(item => item.CountSemicolons());
            }
        }

        public CharactersSurfaceData GetData()
        {
            return (CharactersSurfaceData)data.Clone();
        }

        public void Load()
        {
            data.PercentOfPunctuation = TotalPunctuation / (double)Text.TotalCharacters;
            data.PercentOfSemicolons = TotalSemicolons / (double)Text.TotalCharacters;
            data.PercentOfCommas = TotalCommas / (double)Text.TotalCharacters;
        }
    }
}
