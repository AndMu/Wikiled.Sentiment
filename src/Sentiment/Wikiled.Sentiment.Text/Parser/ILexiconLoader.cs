using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ILexiconLoader
    {
        IEnumerable<string> Supported { get; }

        ISentimentDataHolder GetLexicon(string name);
    }
}