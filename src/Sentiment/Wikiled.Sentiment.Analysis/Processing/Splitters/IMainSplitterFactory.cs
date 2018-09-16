using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public interface IMainSplitterFactory
    {
        IContainerHelper Create(POSTaggerType value, SentimentContext context);
    }
}