using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.Analysis.Processing.Arff
{
    public class UnigramProcessArffFactory : IProcessArffFactory
    {
        public IProcessArff Create(IArffDataSet dataSet) 
        {
            return new UnigramProcessArff(dataSet);
        }
    }
}
