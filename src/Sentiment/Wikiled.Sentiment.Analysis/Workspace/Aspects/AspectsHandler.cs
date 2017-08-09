using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Workspace.Aspects
{
    public class AspectsHandler : IAspectsHandler
    {
        private readonly Func<IWordsHandler> handler;

        private readonly string path;

        private XDocument data;

        public AspectsHandler(string path, Func<IWordsHandler> handler)
        {
            Guard.NotNullOrEmpty(() => path, path);
            Guard.NotNull(() => handler, handler);
            this.path = path;
            this.handler = handler;
            Aspects = NullAspectDectector.Instance;
        }

        public IAspectDectector Aspects { get; private set; }

        public bool CanSave => data != null;

        public void Detect(IParsedReview[] reviews)
        {
            Guard.NotNull(() => reviews, reviews);
            var featureExtractor = handler().AspectFactory.Construct();
            Parallel.ForEach(
                reviews,
                AsyncSettings.DefaultParallel,
                review =>
                {
                    featureExtractor.Process(review);
                });

            Aspects = new AspectDectector(featureExtractor.GetFeatures(100).ToArray(), featureExtractor.GetAttributes(100).ToArray());
            data = handler().AspectFactory.ConstructSerializer().Serialize(Aspects);
        }

        public void Load()
        {
            if (File.Exists(path))
            {
                data = XDocument.Load(path);
                Aspects = handler().AspectFactory.ConstructSerializer().Deserialize(data);
            }
            else
            {
                Aspects = NullAspectDectector.Instance;
            }
        }

        public void Reset()
        {
            Aspects = NullAspectDectector.Instance;
        }

        public void Save()
        {
            data?.XmlSerialize().Save(path);
        }
    }
}
