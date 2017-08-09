using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    public class CharactersSurface : IDataSource
    {
        private readonly CharactersSurfaceData data = new CharactersSurfaceData();

        public CharactersSurface(TextBlock text)
        {
            Guard.NotNull(() => text, text);
            Text = text;
        }

        public void Load()
        {
            data.PercentOfPunctuation = (double)TotalPunctuation / (double)Text.TotalCharacters;
            data.PercentOfSemicolons = (double)TotalSemicolons / (double)Text.TotalCharacters;
            data.PercentOfCommas = (double)TotalCommas / (double)Text.TotalCharacters;
        }

        public TextBlock Text { get; private set; }

        private int TotalPunctuation
        {
            get { return Text.Sentences.Sum(item => item.CountPunctuations()); }
        }

        private int TotalSemicolons
        {
            get { return Text.Sentences.Sum(item => item.CountSemicolons()); }
        }

        private int TotalCommas
        {
            get { return Text.Sentences.Sum(item => item.CountCommas()); }
        }

        /// <summary>
        /// Percentage of all characters that are punctuation characters
        /// </summary>
        [InfoField("Percentage of punctuation")]
        public double PercentOfPunctuation
        {
            get
            {
                return data.PercentOfPunctuation;
            }
        }

        /// <summary>
        /// Percentage of all characters that are semicolons
        /// </summary>
        [InfoField("Percentage of semicolons")]
        public double PercentOfSemicolons
        {
            get
            {
                return data.PercentOfSemicolons;
            }
        }

        /// <summary>
        /// Percentage of all characters that are commas
        /// </summary>
        [InfoField("Percentage of commas")]
        public double PercentOfCommas
        {
            get
            {
                return data.PercentOfCommas;
            }
        }

        public CharactersSurfaceData GetData()
        {
            return (CharactersSurfaceData)data.Clone();
        }
    }
}
