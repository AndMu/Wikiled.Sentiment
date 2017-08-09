using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Workspace.Data.Selection
{
    public interface IFileSelection
    {
        FileSelectionData Configuration { get; }

        IProcessingData Data { get; }
    }
}
