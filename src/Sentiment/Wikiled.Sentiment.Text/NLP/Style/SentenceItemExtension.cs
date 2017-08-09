using System.Linq;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    public static class SentenceItemExtension
    {
        /// <summary>
        /// Is current sentence is question
        /// </summary>
        public static bool IsQuestion(this SentenceItem sentence)
        {
            return sentence.Text.IndexOf('?') >= 0;
        }

        public static int CountPunctuations(this SentenceItem sentence)
        {
            return sentence.Text.Count(char.IsPunctuation);
        }

        public static int CountCharacters(this SentenceItem sentence)
        {
            return sentence.Text.Count(char.IsLetterOrDigit);
        }

        public static int CountSemicolons(this SentenceItem sentence)
        {
            return sentence.Text.Count(item => item == ';');
        }

        public static int CountCommas(this SentenceItem sentence)
        {
            return sentence.Text.Count(item => item == ',');
        }
    }
}
