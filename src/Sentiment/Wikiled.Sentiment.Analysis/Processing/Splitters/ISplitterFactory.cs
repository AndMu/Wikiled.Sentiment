using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public interface ISplitterFactory
    {
        ISplitterHelper Create(POSTaggerType value);
    }
}