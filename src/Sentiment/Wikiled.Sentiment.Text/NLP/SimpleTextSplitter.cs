using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Tokenizer;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleTextSplitter : BaseTextSplitter
    {
        private readonly IWordsHandler wordsHandler;

        public SimpleTextSplitter(IWordsHandler wordsHandler)
            : base(wordsHandler, NullCachedDocumentsSource.Instance)
        {
            Guard.NotNull(() => wordsHandler, wordsHandler);
            this.wordsHandler = wordsHandler;
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            ISentenceTokenizer tokenizer = SentenceTokenizer.Create(wordsHandler, WordsTokenizerFactory.NotWhiteSpace, true, false);
            SimpleWordsExtraction wordsExtraction = new SimpleWordsExtraction(tokenizer);
            Document document = wordsExtraction.GetDocument(request.Document.Text);
            document.Init(request.Document);
            return document;
        }
    }
}