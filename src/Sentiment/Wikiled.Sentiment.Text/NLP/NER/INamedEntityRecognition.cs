using System.Collections.Generic;
using SharpNL.Utility;

namespace Wikiled.Sentiment.Text.NLP.NER
{
    public interface INamedEntityRecognition
    {
        IEnumerable<Span> Tag(string[] words);
    }
}