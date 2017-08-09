using System.IO;
using System.Linq;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser.Cache;

namespace Wikiled.Sentiment.Analysis.Workspace.Data.Selection
{
    public class ArchivedFileSelection : IFileSelection
    {
        public ArchivedFileSelection(string file)
        {
            DocumentPersist persistor = DocumentPersist.CreateStandard(Path.GetDirectoryName(file));
            var data = new ProcessingData();
            var docs = persistor.GetAllDocuments()
                     .Select(item => new SingleProcessingData(item));
            foreach (var doc in docs)
            {
                data.Add(doc.Document.GetPositivity() ?? PositivityType.Positive, doc);
            }

            Data = data;
            Configuration = new FileSelectionData
                            {
                                File = file,
                                Type = FileType.Archived
                            };
        }

        public FileSelectionData Configuration { get; }

        public IProcessingData Data { get; }
    }
}
