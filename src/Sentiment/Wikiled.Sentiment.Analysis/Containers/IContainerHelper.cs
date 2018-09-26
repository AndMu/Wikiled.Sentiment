using Autofac;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public interface IContainerHelper
    {
        IContainer Container { get; }

        SentimentContext Context { get; }

        ITextSplitter GetTextSplitter();

        IWordsHandler GetDataLoader();

        ITestingClient GetTesting(string path = null);

        ITrainingClient GetTraining(string path);
    }
}