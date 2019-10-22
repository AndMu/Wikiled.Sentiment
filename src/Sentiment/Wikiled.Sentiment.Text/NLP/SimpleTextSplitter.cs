using Microsoft.Extensions.Logging;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Parser.Light;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure.Light;
using Wikiled.Text.Analysis.Tokenizer;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleTextSplitter : BaseTextSplitter
    {
        private readonly ISentenceTokenizerFactory sentenceTokenizer;

        public SimpleTextSplitter(ILogger<SimpleTextSplitter> logger, ISentenceTokenizerFactory sentenceTokenizer, ICachedDocumentsSource cache)
            : base(logger, cache)
        {
            this.sentenceTokenizer = sentenceTokenizer;
        }

        protected override LightDocument ActualProcess(ParseRequest request)
        {
            var tokenizer = sentenceTokenizer.Create(true, false);
            var wordsExtraction = new SimpleWordsExtraction(tokenizer);
            LightDocument document = wordsExtraction.GetDocument(request.Document.Text);
            return document;
        }
    }
}