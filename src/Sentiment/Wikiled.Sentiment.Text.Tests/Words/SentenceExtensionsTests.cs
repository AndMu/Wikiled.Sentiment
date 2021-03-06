using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class SentenceExtensionsTests
    {
        [Test]
        public void GetImportant()
        {
            var phrase = new TestPhrase("A");
            var word = new TestWordItem("A")
            {
                IsSentiment = true,
                Parent = phrase
            };

            var word2 = new TestWordItem("B")
            {
                IsStopWord = true,
                Text = "B",
                Parent = phrase
            };

            phrase.Add(word);
            phrase.Add(word2);
            var words = new[] { word, word2 };
            var result = SentenceExtensions.GetImportant(words).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(word, result[0]);
        }
    }
}