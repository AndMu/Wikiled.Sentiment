using System;
using System.Linq;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style.Description
{
    public class StyleExtractor
    {
        private readonly Document document;

        private readonly IWordsHandler handler;

        public StyleExtractor(IWordsHandler handler, Document document)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public DocumentStyle Extract()
        {
            TextBlock text = new TextBlock(handler, document.Sentences.ToArray());
            var style = new DocumentStyle();
            style.Obscrunity = text.VocabularyObscurity.GetData();
            style.CharactersSurface = text.Surface.Characters.GetData();
            style.SentenceSurface = text.Surface.Sentence.GetData();
            style.WordSurface = text.Surface.Words.GetData();
            style.Readability = text.Readability.GetData();
            style.Syntax = text.SyntaxFeatures.GetData();

            foreach (var sentence in document.Sentences.Where(item => item.Words.Count > 0))
            {
                text = new TextBlock(handler, new [] { sentence }, false);
                var sentenceStyle = new SentenceStyle();
                sentenceStyle.Sentence = sentence;
                style.Sentences.Add(sentenceStyle);
                text.Surface.Words.Load();
                sentenceStyle.WordSurface = text.Surface.Words.GetData();
                foreach (var word in sentence.Words)
                {
                    var wordStyle = new WordStyle(word);
                    wordStyle.Inquirer = text.InquirerFinger.GetData(word);
                    var record = handler.NRCDictionary.FindRecord(word);
                    if (record != null)
                    {
                        wordStyle.NRC = record;
                    }

                    if (wordStyle.HasValue)
                    {
                        sentenceStyle.Words.Add(wordStyle);
                    }
                }
            }

            return style;
        }
    }
}
