using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public class ProcessArffFactory : IProcessArffFactory
    {
        public IProcessArff Create(IArffDataSet dataSet)
        {
            return new ProcessArff(dataSet);
        }
    }
}
