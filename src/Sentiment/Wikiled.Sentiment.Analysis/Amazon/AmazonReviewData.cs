using System;
using ProtoBuf;

namespace Wikiled.Sentiment.Analysis.Amazon
{
    [ProtoContract]
    public class AmazonReviewData
    {
        [ProtoMember(1)]
        public int? Helpfulness { get; set; }

        [ProtoMember(2)]
        public int? TotalHelpfulnessVotes { get; set; }

        [ProtoMember(3)]
        public double Score { get; set; }

        [ProtoMember(4)]
        public int Time { get; set; }

        [ProtoMember(5)]
        public string Id { get; set; }

        [ProtoMember(6)]
        public string UserId { get; set; }

        [ProtoMember(7)]
        public string ProductId { get; set; }

        [ProtoMember(8)]
        public DateTime Date { get; set; }
    }
}
