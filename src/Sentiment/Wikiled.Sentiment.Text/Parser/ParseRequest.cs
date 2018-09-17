using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public class ParseRequest
    {
        public ParseRequest(string text)
            : this(new Document(text))
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
            }
        }

        public ParseRequest(Document document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public Document Document { get; }
    }
}
