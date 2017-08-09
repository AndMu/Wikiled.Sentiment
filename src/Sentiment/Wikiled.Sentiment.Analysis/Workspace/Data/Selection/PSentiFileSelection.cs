using System;
using Wikiled.Sentiment.Analysis.Processing;

namespace Wikiled.Sentiment.Analysis.Workspace.Data.Selection
{
    public class PSentiFileSelection : IFileSelection
    {
        private readonly Lazy<IProcessingData> data;

        public PSentiFileSelection(string file)
        {
            data = new Lazy<IProcessingData>(() => ProcessingDataExtension.Load(file));
            Configuration = new FileSelectionData
            {
                File = file,
                Type = FileType.PSenti
            };
        }

        public FileSelectionData Configuration { get; }

        public IProcessingData Data => data.Value;
    }
}
