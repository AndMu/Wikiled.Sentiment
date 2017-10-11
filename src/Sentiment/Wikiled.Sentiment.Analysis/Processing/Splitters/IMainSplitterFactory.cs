using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public interface IMainSplitterFactory
    {
        ISplitterHelper Create(POSTaggerType value);
    }
}