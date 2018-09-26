using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.NLP
{
    [TestFixture]
    public class SimpleTextSplitterTests
    {
        [Test]
        public async Task Process()
        {
            var splitter = ActualWordsHandler.InstanceSimple.TextSplitter;
            string sentence = "By default, the application is set to search for new virus definitions daily, but you always can use the scheduling tool to change this.";
            string sentence2 = "Should a virus create serious system problems, AVG creates a rescue disk to scan your computer in MS-DOS mode.";
            var result = await splitter.Process(new ParseRequest(sentence + " " + sentence2)).ConfigureAwait(false);
            var data = ActualWordsHandler.InstanceSimple.Container.Container.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();

            Assert.AreEqual(2, data.Sentences.Count);
            Assert.AreEqual(24, data.Sentences[0].Occurrences.Count());
            Assert.AreEqual(13, data.Sentences[0].Occurrences.GetImportant().Count());
            Assert.AreEqual(3, data.Sentences[0].Parts.Count());

            Assert.AreEqual(19, data.Sentences[1].Occurrences.Count());
            Assert.AreEqual(13, data.Sentences[1].Occurrences.GetImportant().Count());
            Assert.AreEqual(2, data.Sentences[1].Parts.Count());
        }
    }
}
