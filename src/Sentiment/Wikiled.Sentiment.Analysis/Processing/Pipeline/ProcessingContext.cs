using Wikiled.Sentiment.Text.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Pipeline
{
    public class ProcessingContext
    {
        public ProcessingContext(Document original, Document processed, IParsedReview review)
        {
            Original = original;
            Processed = processed;
            Review = review;
        }

        public Document Original { get; }

        public Document Processed { get; set; }

        public IParsedReview Review { get; }
    }
}
