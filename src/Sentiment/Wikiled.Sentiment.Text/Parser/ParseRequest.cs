using Wikiled.Common.Arguments;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public class ParseRequest
    {
        public ParseRequest(string text)
            : this(new Document(text))
        {
            Guard.NotNullOrEmpty(() => text, text);
        }

        public ParseRequest(Document document)
        {
            Guard.NotNull(() => document, document);
            Document = document;
        }

        public Document Document { get; }
    }
}
