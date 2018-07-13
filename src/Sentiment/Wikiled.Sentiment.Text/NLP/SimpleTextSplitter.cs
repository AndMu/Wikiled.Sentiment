using System;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleTextSplitter : BaseTextSplitter
    {
        private readonly IWordsHandler wordsHandler;

        public SimpleTextSplitter(IWordsHandler wordsHandler)
            : base(wordsHandler, NullCachedDocumentsSource.Instance)
        {
            this.wordsHandler = wordsHandler ?? throw new ArgumentNullException(nameof(wordsHandler));
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            ISentenceTokenizer tokenizer = SentenceTokenizer.Create(wordsHandler.PosTagger, WordsTokenizerFactory.NotWhiteSpace, true, false);
            SimpleWordsExtraction wordsExtraction = new SimpleWordsExtraction(tokenizer);
            Document document = wordsExtraction.GetDocument(request.Document.Text);
            return document;
        }
    }
}