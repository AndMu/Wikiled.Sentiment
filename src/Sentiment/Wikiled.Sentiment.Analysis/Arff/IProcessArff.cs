using System.Threading.Tasks;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public interface IProcessArff
    {
        void PopulateArff(IParsedReview[] processing, PositivityType positivity);

        void PopulateArff(IParsedReview processings, PositivityType positivity);

        Task CleanupDataHolder(int minReviewSize, int minOccurences);
    }
}