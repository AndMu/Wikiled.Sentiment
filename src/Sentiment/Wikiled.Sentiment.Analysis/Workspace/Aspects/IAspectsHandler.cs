using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Workspace.Aspects
{
    public interface IAspectsHandler
    {
        IAspectDectector Aspects { get; }

        bool CanSave { get; }

        void Detect(IParsedReview[] reviews);

        void Load();

        void Reset();

        void Save();
    }
}
