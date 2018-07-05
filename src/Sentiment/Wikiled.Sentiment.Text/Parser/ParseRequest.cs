using Wikiled.Common.Arguments;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public class ParseRequest
    {
        public ParseRequest(string text)
            : this(new Document(text))
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new System.ArgumentException("message", nameof(text));
            }
        }

        public ParseRequest(Document document)
        {
            Document = document ?? throw new System.ArgumentNullException(nameof(document));
        }

        public Document Document { get; }
    }
}
