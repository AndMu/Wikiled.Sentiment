using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface IExtendedWords
    {
        IEnumerable<(string Word, int Sentiment)> GetSentiments();

        IEnumerable<(string Word, string Replacement)> GetReplacements();
         
        bool HasAlternative(string word, out string alternative);
    }
}