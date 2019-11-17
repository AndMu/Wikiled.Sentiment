using System;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsingDocumentHolder : IParsedDocumentHolder
    {
        private readonly ITextSplitter splitter;

        private readonly Document original;

        private readonly IWordFactory factory;

        private readonly IContextSentenceRepairHandler repairHandler;

        public ParsingDocumentHolder(ITextSplitter splitter,
                                     IWordFactory factory,
                                     IContextSentenceRepairHandler repairHandler,
                                     Document doc)
        {
            this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            original = doc ?? throw new ArgumentNullException(nameof(doc));
            this.repairHandler = repairHandler ?? throw new ArgumentNullException(nameof(repairHandler));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ParsingDocumentHolder(ITextSplitter splitter,
                                     IWordFactory factory,
                                     IContextSentenceRepairHandler repairHandler,
                                     SingleProcessingData doc)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.repairHandler = repairHandler ?? throw new ArgumentNullException(nameof(repairHandler));
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

            original.Text = repairHandler.Repair(original.Text);
            var document = await splitter.Process(new ParseRequest(original)).ConfigureAwait(false);
            var result = document.Construct(factory);
            result.Status = Status.Parsed;
            return result;
        }
    }
}
