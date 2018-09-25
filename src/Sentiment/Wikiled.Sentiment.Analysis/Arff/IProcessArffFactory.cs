using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public interface IProcessArffFactory
    {
        IProcessArff Create(IArffDataSet dataSet);
    }
}