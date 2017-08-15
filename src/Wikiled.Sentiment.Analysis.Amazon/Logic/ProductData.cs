using ProtoBuf;

namespace Wikiled.Sentiment.Analysis.Amazon.Logic
{
    [ProtoContract]
    public class ProductData
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public double? Price { get; set; }

        [ProtoMember(4)]
        public ProductCategory Category { get; set; }

    }
}
