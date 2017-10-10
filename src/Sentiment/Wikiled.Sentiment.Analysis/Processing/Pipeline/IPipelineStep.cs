using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public interface IPipelineStep
    {
        Task<IParsedReview> AdditionalProcessing(IParsedDocumentHolder reviewHolder);
    }
}