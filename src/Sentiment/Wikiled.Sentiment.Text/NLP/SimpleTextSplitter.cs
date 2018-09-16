﻿using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;

namespace Wikiled.Sentiment.Text.NLP
{
    public class SimpleTextSplitter : BaseTextSplitter
    {
        private readonly ISentenceTokenizerFactory sentenceTokenizer;

        public SimpleTextSplitter(ISentenceTokenizerFactory sentenceTokenizer)
            : base(NullCachedDocumentsSource.Instance)
        {
            this.sentenceTokenizer = sentenceTokenizer;
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            var tokenizer = sentenceTokenizer.Create(true, false);
            SimpleWordsExtraction wordsExtraction = new SimpleWordsExtraction(tokenizer);
            Document document = wordsExtraction.GetDocument(request.Document.Text);
            return document;
        }
    }
}