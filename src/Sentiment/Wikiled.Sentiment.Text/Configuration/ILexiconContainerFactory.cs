using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Configuration
{
    public interface ILexiconContainerFactory
    {
        IWordsHandler Construct();
    }
}