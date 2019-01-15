using System;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public class AsyncParsingDocumentHolder : IParsedDocumentHolder
    {
        private readonly Task<IParsedDocumentHolder> delayed;

        public AsyncParsingDocumentHolder(Task<IParsedDocumentHolder> delayed)
        {
            this.delayed = delayed ?? throw new ArgumentNullException(nameof(delayed));
        }

        public async Task<Document> GetParsed()
        {
            var resolved = await delayed.ConfigureAwait(false);
            return await resolved.GetParsed().ConfigureAwait(false); 
        }

        public async Task<Document> GetOriginal()
        {
            var resolved = await delayed.ConfigureAwait(false);
            return await resolved.GetOriginal().ConfigureAwait(false);
        }
    }
}
