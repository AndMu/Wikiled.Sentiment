using SharpNL.Utility;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    internal class SentenceData
    {
        public string Text { get; set; }

        public string[] Tokens { get; set; }

        public string[] Tags { get; set; }

        public Span[] Chunks { get; set; }
    }
}
