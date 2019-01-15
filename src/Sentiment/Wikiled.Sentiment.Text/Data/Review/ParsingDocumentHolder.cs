using System;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsingDocumentHolder : IParsedDocumentHolder
    {
        private readonly ITextSplitter splitter;

        private Document original;

        public ParsingDocumentHolder(ITextSplitter splitter, Document doc)
        {
            this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            original = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        public ParsingDocumentHolder(ITextSplitter splitter, SingleProcessingData doc)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            original = new Document(doc.Text);
            original.DocumentTime = doc.Date;
            original.Stars = doc.Stars;
            original.Author = doc.Author;
            original.Id = doc.Id;
        }

        public Task<Document> GetOriginal()
        {
            return Task.FromResult(original);
        }

        public async Task<Document> GetParsed()
        {
            var document = await splitter.Process(new ParseRequest(await GetOriginal().ConfigureAwait(false))).ConfigureAwait(false);
            document.Status = Status.Parsed;
            return document;
        }
    }
}
