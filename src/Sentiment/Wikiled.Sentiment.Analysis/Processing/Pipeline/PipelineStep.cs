using System;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public class PipelineStep : IPipelineStep
    {
        public Task<IParsedReview> AdditionalProcessing(IParsedDocumentHolder reviewHolder)
        {
            throw new NotImplementedException();
        }
    }
}
