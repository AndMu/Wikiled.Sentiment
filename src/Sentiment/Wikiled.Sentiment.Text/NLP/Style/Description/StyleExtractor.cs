using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.NLP.NRC;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.NLP.Style.Description
{
    public class StyleExtractor
    {
        private readonly Document document;

        private readonly IWordsHandler handler;

        private readonly IInquirerManager inquirer;

        public StyleExtractor(IInquirerManager inquirer, IWordsHandler handler, Document document)
        {
            Guard.NotNull(() => handler, handler);
            Guard.NotNull(() => inquirer, inquirer);
            Guard.NotNull(() => document, document);
            this.inquirer = inquirer;
            this.document = document;
            this.inquirer = inquirer;
            this.handler = handler;
        }

        public DocumentStyle Extract()
        {
            TextBlock text = new TextBlock(inquirer, handler, document.Sentences.ToArray());
            var style = new DocumentStyle();
            style.Obscrunity = text.VocabularyObscurity.GetData();
            style.CharactersSurface = text.Surface.Characters.GetData();
            style.SentenceSurface = text.Surface.Sentence.GetData();
            style.WordSurface = text.Surface.Words.GetData();
            style.Readability = text.Readability.GetData();
            style.Syntax = text.SyntaxFeatures.GetData();

            foreach (var sentence in document.Sentences.Where(item => item.Words.Count > 0))
            {
                text = new TextBlock(inquirer, handler, new [] { sentence }, false);
                var sentenceStyle = new SentenceStyle();
                sentenceStyle.Sentence = sentence;
                style.Sentences.Add(sentenceStyle);
                text.Surface.Words.Load();
                sentenceStyle.WordSurface = text.Surface.Words.GetData();
                foreach (var word in sentence.Words)
                {
                    var wordStyle = new WordStyle();
                    sentenceStyle.Words.Add(wordStyle);
                    wordStyle.Inquirer = text.InquirerFinger.GetData(word);
                    var record = NRCDictionary.Instance.FindRecord(word);
                    if (record != null)
                    {
                        wordStyle.NRC = record;
                    }
                }
            }

            return style;
        }
    }
}
