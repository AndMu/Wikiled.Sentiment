using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.Analysis.Processing.Arff
{
    public interface IProcessArffFactory
    {
        IProcessArff Create(IArffDataSet dataSet);
    }
}