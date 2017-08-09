using Wikiled.Sentiment.Text.Features.Results;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Features
{
    public interface IFeatureIndicator
    {
        IIdentificationResult CheckIndication(IWordItem word);
    }
}