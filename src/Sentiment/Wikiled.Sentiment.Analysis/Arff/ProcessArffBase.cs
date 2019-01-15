using System;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Arff.Extensions;
using Wikiled.Arff.Logic;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public abstract class ProcessArffBase : IProcessArff
    {
        protected ProcessArffBase(IArffDataSet dataSet)
        {
            DataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
        }

        public IArffDataSet DataSet { get; }

        public async Task CleanupDataHolder(int minReviewSize, int minOccurences)
        {
            await DataSet.CompactHeader(minOccurences).ConfigureAwait(false);
            DataSet.CompactReviews(minReviewSize);
        }

        public void PopulateArff(IParsedReview[] processings, PositivityType positivity)
        {
            if (processings is null)
            {
                throw new ArgumentNullException(nameof(processings));
            }

            if (processings.Length == 0 || processings.Any(x => x is null))
            {
                throw new ArgumentException("Value cannot be an empty collection and not null.", nameof(processings));
            }

            Parallel.ForEach(
                processings,
                AsyncSettings.DefaultParallel,
                review => PopulateArff(review, positivity));
        }

        public abstract void PopulateArff(IParsedReview current, PositivityType positivity);
    }
}