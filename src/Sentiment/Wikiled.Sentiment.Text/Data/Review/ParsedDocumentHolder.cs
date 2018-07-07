using System;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsedDocumentHolder : IParsedDocumentHolder
    {
        private readonly Document parsed;

        public ParsedDocumentHolder(Document original, Document parsed)
        {
            this.parsed = parsed ?? throw new ArgumentNullException(nameof(parsed));
            Original = original ?? throw new ArgumentNullException(nameof(original));
        }

        public Document Original { get; }

        public Task<Document> GetParsed()
        {
            return Task.FromResult(parsed);
        }
    }
}
