using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public interface IMainSplitterFactory
    {
        IContainerHelper Create(POSTaggerType value);
    }
}