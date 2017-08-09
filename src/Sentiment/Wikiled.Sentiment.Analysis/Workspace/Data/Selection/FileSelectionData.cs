using System;

namespace Wikiled.Sentiment.Analysis.Workspace.Data.Selection
{
    public class FileSelectionData
    {
        public IFileSelection Construct()
        {
            switch (Type)
            {
                case FileType.PSenti:
                    return new PSentiFileSelection(File);
                case FileType.Archived:
                    return new ArchivedFileSelection(File);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string File { get; set; }

        public FileType Type { get; set; }
    }
}
