using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class ParsedDocumentHolder : IParsedDocumentHolder
    {
        private readonly Document parsed;

        public ParsedDocumentHolder(Document original, Document parsed)
        {
            Guard.NotNull(() => parsed, parsed);
            Guard.NotNull(() => original, original);
            this.parsed = parsed;
            Original = original;
        }

        public Document Original { get; }

        public Task<Document> GetParsed()
        {
            return Task.FromResult(parsed);
        }
    }
}
