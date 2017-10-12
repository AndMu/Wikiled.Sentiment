using System.Threading.Tasks;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Processing.Arff
{
    public interface IProcessArff
    {
        void PopulateArff(IParsedReview[] processing, PositivityType positivity);

        void PopulateArff(IParsedReview processings, PositivityType positivity);

        Task CleanupDataHolder(int minReviewSize, int minOccurences);

        void Normalize(NormalizationType normalization);
    }
}