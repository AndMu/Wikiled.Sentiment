using Autofac;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing.Splitters
{
    public interface IContainerHelper
    {
        IContainer Container { get; }

        ITextSplitter GetTextSplitter();

        IWordsHandler GetDataLoader();

        void ChangeWordsHandler(IAspectDectector detector);

        IParsedReviewManager Resolve(Document document);
    }
}