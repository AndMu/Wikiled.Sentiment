using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public interface IClientContext
    {
        IProcessingPipeline Pipeline { get;}

        SessionContext Context { get; }

        IAspectSerializer AspectSerializer { get; }

        INRCDictionary NrcDictionary { get; }
    }
}
