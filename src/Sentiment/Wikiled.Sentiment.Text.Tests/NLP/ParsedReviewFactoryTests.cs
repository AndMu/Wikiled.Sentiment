using System.Linq;
using NUnit.Framework;
using Wikiled.Common.Serialization;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.NLP
{
    [TestFixture]
    public class ParsedReviewFactoryTests
    {
        private Document document;

        [SetUp]
        public void Setup()
        {
            document = new Document("Test");
            document.Sentences.Add(new SentenceItem("Test"));
            document.Sentences[0].Words.Add(WordExFactory.Construct(new TestWordItem { Text = "One" }));
            document.Sentences[0].Words.Add(WordExFactory.Construct(new TestWordItem { Text = "Two" }));
            document.Sentences[0].Words.Add(WordExFactory.Construct(new TestWordItem { Text = "Three" }));
        }

        [Test]
        public void TestPhraseConversion()
        {
            var factory = (ParsedReviewManager)ActualWordsHandler.InstanceSimple.Container.Resolve(document);
            document.Sentences[0].Words[0].Phrase = "1";
            document.Sentences[0].Words[1].Phrase = "1";

            var review = factory.Create();
            var phrases = review.Sentences[0].Occurrences.GetPhrases().ToArray();
            Assert.AreEqual(1, phrases.Length);

            var data = document.XmlSerialize();
            var doc = data.XmlDeserialize<Document>();
            factory = (ParsedReviewManager)ActualWordsHandler.InstanceSimple.Container.Resolve(doc);

            review = factory.Create();
            phrases = review.Sentences[0].Occurrences.GetPhrases().ToArray();
            Assert.AreEqual(1, phrases.Length);
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(1, review.Sentences[0].Parts.Count());
            Assert.AreEqual(3, review.Sentences[0].Occurrences.Count());
        }

        [Test]
        public void AddWord()
        {
            var factory = (ParsedReviewManager)ActualWordsHandler.InstanceSimple.Container.Resolve(document);
            var review = factory.Create();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(1, review.Sentences[0].Parts.Count());
            Assert.AreEqual(3, review.Sentences[0].Occurrences.Count());
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void AddWordConjunction(int index, int parts)
        {
            (document.Sentences[0].Words[index]).Type = ",";
            var factory = (ParsedReviewManager)ActualWordsHandler.InstanceSimple.Container.Resolve(document);
            var review = factory.Create();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(parts, review.Sentences[0].Parts.Count());
            Assert.AreEqual(2, review.Sentences[0].Occurrences.Count());
        }

        [Test]
        public void AddWordTwoConjunctionEmpty()
        {
            document.Sentences[0].Words[1].Type = ",";
            document.Sentences[0].Words[2].Type = ",";

            var factory = (ParsedReviewManager)ActualWordsHandler.InstanceSimple.Container.Resolve(document);
            var review = factory.Create();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(1, review.Sentences[0].Parts.Count());
            Assert.AreEqual(1, review.Sentences[0].Parts.First().Occurrences.GetImportant().Count());
            Assert.AreEqual(1, review.Sentences[0].Occurrences.Count());
        }

        [Test]
        public void AddPhraseWord()
        {
            var factory = (ParsedReviewManager)ActualWordsHandler.InstanceSimple.Container.Resolve(document);
            document.Sentences[0].Words[0].Phrase = "1";
            document.Sentences[0].Words[1].Phrase = "1";

            var review = factory.Create();
            Assert.AreEqual(1, review.Sentences.Count);
            Assert.AreEqual(3, review.Sentences[0].Occurrences.Count());
            var phrases = review.Items.GetPhrases().ToArray();

            Assert.AreEqual("one two", phrases[0].Text);
            Assert.AreEqual(2, phrases[0].AllWords.Count());
            var words = review.Items.ToArray();
            Assert.AreEqual(phrases[0], words[0].Parent);
            Assert.AreEqual(phrases[0], words[1].Parent);
            Assert.IsNull(review.Items.ToArray()[2].Parent);
        }
    }
}
