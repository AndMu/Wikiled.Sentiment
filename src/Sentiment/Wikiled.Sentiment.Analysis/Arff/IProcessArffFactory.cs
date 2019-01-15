using Wikiled.Arff.Logic;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public interface IProcessArffFactory
    {
        IProcessArff Create(IArffDataSet dataSet);
    }
}