using Wikiled.Arff.Logic;

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
