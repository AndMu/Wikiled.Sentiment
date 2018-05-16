using System.Threading.Tasks;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsingDocumentHolder : IParsedDocumentHolder
    {
        private readonly ITextSplitter splitter;

        public ParsingDocumentHolder(ITextSplitter splitter, Document doc)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => doc, doc);
            this.splitter = splitter;
            Original = doc;
        }

        public ParsingDocumentHolder(ITextSplitter splitter, SingleProcessingData doc)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => doc, doc);
            this.splitter = splitter;
            Original = new Document(doc.Text);
            Original.DocumentTime = doc.Date;
            Original.Stars = doc.Stars;
        }

        public Document Original { get; }

        public Task<Document> GetParsed()
        {
            return splitter.Process(new ParseRequest(Original) { Date = Original.DocumentTime });
        }
    }
}
