using System.IO;

namespace Wikiled.Sentiment.Text.Config
{
    public class LexiconConfig : ILexiconConfig
    {
        public string Lexicon { get; set; }

        public string Resources { get; set; }

        public string Remote { get; set; }

        public string FullLexiconPath => Path.Combine(Resources, Lexicon);
    }
}
