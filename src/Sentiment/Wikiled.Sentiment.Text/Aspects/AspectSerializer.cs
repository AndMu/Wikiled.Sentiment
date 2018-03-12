using System.Linq;
using System.Xml.Linq;
using Wikiled.Common.Arguments;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.Text.Aspects.Data;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectSerializer : IAspectSerializer
    {
        private readonly IWordsHandler handler;

        public AspectSerializer(IWordsHandler handler)
        {
            Guard.NotNull(() => handler, handler);
            this.handler = handler;
        }

        public XDocument Serialize(IMainAspectHandler aspectHandler)
        {
            Guard.NotNull(() => aspectHandler, aspectHandler);
            return Serialize(aspectHandler.GetFeatures(100).ToArray(), aspectHandler.GetAttributes(100).ToArray());
        }

        public XDocument Serialize(IWordItem[] aspects, IWordItem[] attributes)
        {
            Guard.NotNull(() => aspects, aspects);
            Guard.NotNull(() => attributes, attributes);
            AspectData data = new AspectData();
            data.Aspects = aspects.Select(item => item.Text).ToArray();
            data.Attributes = attributes.Select(item => item.Text).ToArray();
            return data.XmlSerialize();
        }

        public XDocument Serialize(IAspectDectector dectector)
        {
            Guard.NotNull(() => dectector, dectector);
            return Serialize(dectector.AllAttributes.ToArray(), dectector.AllFeatures.ToArray());
        }

        public IAspectDectector Deserialize(XDocument document)
        {
            Guard.NotNull(() => document, document);
            var data = document.XmlDeserialize<AspectData>();
            var aspects = data.Aspects.Select(item => handler.WordFactory.CreateWord(item, "NN")).ToArray();
            var attributes = data.Attributes.Select(item => handler.WordFactory.CreateWord(item, "JJ")).ToArray();
            return new AspectDectector(aspects, attributes);
        }
    }
}
