using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsingDocumentHolder : IParsedDocumentHolder
    {
        private readonly SingleProcessingData doc;

        private readonly ITextSplitter splitter;

        public ParsingDocumentHolder(ITextSplitter splitter, SingleProcessingData doc)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => doc, doc);
            this.splitter = splitter;
            this.doc = doc;
            Original = doc.Document;
        }

        public Document Original { get; }

        public Task<Document> GetParsed()
        {
            return splitter.Process(new ParseRequest(doc.Document) { Date = doc.Date });
        }
    }
}
