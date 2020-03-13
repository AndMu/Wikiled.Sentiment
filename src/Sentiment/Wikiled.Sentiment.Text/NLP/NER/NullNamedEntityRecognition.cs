using System.Collections.Generic;
using SharpNL.Utility;

namespace Wikiled.Sentiment.Text.NLP.NER
{
    public class NullNamedEntityRecognition : INamedEntityRecognition
    {
        public IEnumerable<Span> Tag(string[] words)
        {
            yield break;
        }
    }
}
