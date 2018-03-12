using System.Linq;
using System.Threading.Tasks;
using Wikiled.Arff.Extensions;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Processing.Arff
{
    public abstract class ProcessArffBase : IProcessArff
    {
        protected ProcessArffBase(IArffDataSet dataSet)
        {
            Guard.NotNull(() => dataSet, dataSet);
            DataSet = dataSet;
        }

        public IArffDataSet DataSet { get; }

        public async Task CleanupDataHolder(int minReviewSize, int minOccurences)
        {
            await DataSet.CompactHeader(minOccurences).ConfigureAwait(false);
            DataSet.CompactReviews(minReviewSize);
        }

        public void Normalize(NormalizationType type)
        {
            DataSet.Normalize(type);
        }

        public void PopulateArff(IParsedReview[] processings, PositivityType positivity)
        {
            Guard.IsValid(() => processings, processings, item => item.All(x => x != null), "All reviews should be not null");
            Parallel.ForEach(
                processings,
                AsyncSettings.DefaultParallel,
                review => PopulateArff(review, positivity));
        }

        public abstract void PopulateArff(IParsedReview current, PositivityType positivity);
    }
}