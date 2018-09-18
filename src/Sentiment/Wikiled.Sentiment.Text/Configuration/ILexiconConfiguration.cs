using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Configuration
{
    public interface ILexiconConfiguration
    {
        string LexiconPath { get; }

        string ResourcePath { get; }
    }
}