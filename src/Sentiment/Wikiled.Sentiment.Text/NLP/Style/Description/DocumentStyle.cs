using System.Collections.Generic;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style.Description
{
    public class DocumentStyle
    {
        public DocumentStyle()
        {
            Sentences = new List<SentenceStyle>();
        }

        public Document Document { get; set; }

        public List<SentenceStyle> Sentences { get; set; }

        public AllObscrunityData Obscrunity { get; set; }

        public CharactersSurfaceData CharactersSurface { get; set; }

        public SentenceSurfaceData SentenceSurface { get; set; }

        public WordSurfaceData WordSurface { get; set; }

        public ReadabilityData Readability { get; set; }

        public SyntaxData Syntax { get; set; }
    }
}
