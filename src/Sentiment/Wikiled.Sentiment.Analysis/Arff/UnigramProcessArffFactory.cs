using Wikiled.Arff.Persistence;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public class UnigramProcessArffFactory : IProcessArffFactory
    {
        public IProcessArff Create(IArffDataSet dataSet) 
        {
            return new UnigramProcessArff(dataSet);
        }
    }
}
