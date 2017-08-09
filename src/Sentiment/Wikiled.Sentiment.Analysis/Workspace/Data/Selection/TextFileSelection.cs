using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Workspace.Data.Selection
{
    public class TextFileSelection: IFileSelection
    {
        public TextFileSelection(string file)
        {
            Configuration = new FileSelectionData
            {
                File = file,
                Type = FileType.Text
            };
        }

        public FileSelectionData Configuration { get; private set; }

        public IProcessingData Data { get; private set; }
    }
}
