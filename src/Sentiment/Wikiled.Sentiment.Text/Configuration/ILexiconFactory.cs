using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Configuration
{
    public interface ILexiconFactory : IConfigurationFactory
    {
        IWordsHandler WordsHandler { get; }
    }
}