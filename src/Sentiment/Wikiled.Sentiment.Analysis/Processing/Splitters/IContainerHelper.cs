using Autofac;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public interface IContainerHelper
    {
        IContainer Container { get; }

        SentimentContext Context { get; }

        ITextSplitter GetTextSplitter();

        IWordsHandler GetDataLoader();

        IParsedReviewManager Resolve(Document document, ISentimentDataHolder lexicon = null);
    }
}