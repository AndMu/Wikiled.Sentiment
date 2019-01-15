using System;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsedDocumentHolder : IParsedDocumentHolder
    {
        private readonly Document parsed;

        private readonly Document original;

        public ParsedDocumentHolder(Document original, Document parsed)
        {
            this.parsed = parsed ?? throw new ArgumentNullException(nameof(parsed));
            this.original = original ?? throw new ArgumentNullException(nameof(original));
        }

        public Task<Document> GetOriginal()
        {
            return Task.FromResult(original);
        }

        public Task<Document> GetParsed()
        {
            return Task.FromResult(parsed);
        }
    }
}
