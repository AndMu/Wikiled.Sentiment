using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Tokenizer;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public class SimpleWordsExtraction : IWordsExtraction
    {
        private readonly ISentenceTokenizer tokenizer;

        public SimpleWordsExtraction(ISentenceTokenizer tokenizer)
        {
            Guard.NotNull(() => tokenizer, tokenizer);
            this.tokenizer = tokenizer;
        }

        public Document GetDocument(string text)
        {
            Document document = new Document(text);
            foreach (var sentenceLevel in tokenizer.Parse(text))
            {
                ProcessSentence(document, sentenceLevel.SentenceText, sentenceLevel.GetWordItems());
            }

            return document;
        }

        public Document GetDocumentBySentences(string text, params ISentence[] sentences)
        {
            Document document = new Document(text);
            foreach (var sentence in sentences)
            {
                IEnumerable<WordEx> words = sentence.Occurrences.GetImportant().Select(WordExFactory.Construct);
                ProcessSentence(document, sentence.Text, words);
            }

            return document;
        }

        private static void ProcessSentence(Document document, string sentence, IEnumerable<WordEx> words)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return;
            }

            var currentSentence = new SentenceItem(sentence);
            foreach (WordEx item in words)
            {
                currentSentence.Add(item);
            }

            if (currentSentence.Words.Count > 0)
            {
                document.Sentences.Add(currentSentence);
            }
        }
    }
}
