using Wikiled.Arff.Logic;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Light;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class DocumentExtension
    {
        public static Document Construct(this LightDocument document, IWordFactory factory)
        {
            var result = new Document(document.Text);
            result.Author = document.Author;
            result.DocumentTime = document.DocumentTime;
            result.Id = document.Id;
            document.Title = document.Title;

            foreach (var sentence in document.Sentences)
            {
                var resultSentence = new SentenceItem(sentence.Text);
                result.Add(resultSentence);

                foreach (var word in sentence.Words)
                {
                    var item = factory.CreateWord(word.Text, word.Tag);
                    resultSentence.Add(new WordEx(item));
                }
            }

            return result;
        }

        public static PositivityType? GetPositivity(this Document document)
        {
            if (document?.Stars == null)
            {
                return null;
            }

            return document.Stars > 3 ? PositivityType.Positive : PositivityType.Negative;
        }
    }
}
