using System;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsingDocumentHolder : IParsedDocumentHolder
    {
        private readonly ITextSplitter splitter;

        public ParsingDocumentHolder(ITextSplitter splitter, Document doc)
        {
            this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            Original = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        public ParsingDocumentHolder(ITextSplitter splitter, SingleProcessingData doc)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            Original = new Document(doc.Text);
            Original.DocumentTime = doc.Date;
            Original.Stars = doc.Stars;
            Original.Author = doc.Author;
            Original.Id = doc.Id;
        }

        public Document Original { get; }
        
        public async Task<Document> GetParsed()
        {
            var document = await splitter.Process(new ParseRequest(Original)).ConfigureAwait(false);
            document.Status = Status.Parsed;
            return document;
        }
    }
}
