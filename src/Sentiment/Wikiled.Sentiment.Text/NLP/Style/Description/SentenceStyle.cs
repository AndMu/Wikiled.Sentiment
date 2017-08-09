using System.Collections.Generic;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style.Description
{
    public class SentenceStyle
    {
        public SentenceStyle()
        {
            Words = new List<WordStyle>();
        }

        public List<WordStyle> Words { get; set; }

        public WordSurfaceData WordSurface { get; set; }

        public SentenceItem Sentence { get; set; }
    }
}
