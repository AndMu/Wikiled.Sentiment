using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Features.Results
{
    internal class IdentificationResult : IIdentificationResult
    {
        public static readonly IdentificationResult Empty = new IdentificationResult(new DetectionItem[] {});

        public IdentificationResult(IEnumerable<DetectionItem> blocks)
        {
            Blocks = blocks;
        }

        public IEnumerable<DetectionItem> Blocks { get; }
    }
}
