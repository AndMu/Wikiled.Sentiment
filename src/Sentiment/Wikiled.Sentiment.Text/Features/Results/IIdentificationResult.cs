using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Features.Results
{
    public interface IIdentificationResult
    {
        IEnumerable<DetectionItem> Blocks { get; }
    }
}
