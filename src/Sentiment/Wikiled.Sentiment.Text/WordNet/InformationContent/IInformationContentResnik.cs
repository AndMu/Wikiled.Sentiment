
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Text.WordNet.InformationContent
{
    public interface IInformationContentResnik
    {
        double GetIC(SynSet synSet);

        double GetFrequency(SynSet synSet);

        double TotalNouns { get; }

        double TotalVerbs { get; }
    }
}