using System;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class ClientContext : IClientContext
    {
        public ClientContext(IProcessingPipeline pipeline, SessionContext context, IAspectSerializer aspectSerializer, INRCDictionary nrcDictionary)
        {
            Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            AspectSerializer = aspectSerializer ?? throw new ArgumentNullException(nameof(aspectSerializer));
            NrcDictionary = nrcDictionary ?? throw new ArgumentNullException(nameof(nrcDictionary));
        }

        public IProcessingPipeline Pipeline { get; }

        public SessionContext Context { get; }

        public IAspectSerializer AspectSerializer { get; }

        public INRCDictionary NrcDictionary { get; }
    }
}
