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

namespace Wikiled.Sentiment.Text.Tests.OpenNLP
{
    [TestFixture]
    public class OpenNLPTextSplitterTests
    {
        private ITextSplitter splitter;

        [SetUp]
        public void Setup()
        {
            splitter = ActualWordsHandler.InstanceOpen.TextSplitter;
        }

        [Test]
        public async Task Process()
        {
            var sentence = "By default, the application is set to search for new virus definitions daily, but you always can use the scheduling tool to change this.";
            var sentence2 = "Should a virus create serious system problems, AVG creates a rescue disk to scan your computer in MS-DOS mode.";
            var result = await splitter.Process(new ParseRequest(sentence + ". " + sentence2)).ConfigureAwait(false);
            var data = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();
            Assert.AreEqual(2, data.Sentences.Count);
            Assert.AreEqual(24, data.Sentences[0].Occurrences.Count());
            Assert.AreEqual(12, data.Sentences[0].Occurrences.GetImportant().Count());
            Assert.AreEqual(3, data.Sentences[0].Parts.Count());

            Assert.AreEqual(19, data.Sentences[1].Occurrences.Count());
            Assert.AreEqual(13, data.Sentences[1].Occurrences.GetImportant().Count());
            Assert.AreEqual(2, data.Sentences[1].Parts.Count());
        }

        [Test]
        public async Task SentenceWithSymbols()
        {
            var result = await splitter.Process(new ParseRequest("Woo! Score one for the penny-pinchers!")).ConfigureAwait(false);
            var data = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();
            Assert.AreEqual(2, data.Sentences.Count);
            Assert.AreEqual(1, data.Sentences[0].Occurrences.Count());
            Assert.AreEqual(5, data.Sentences[1].Occurrences.Count());
        }

        [Test]
        public async Task Test()
        {
            var txt =
                "1980 was certainly a year for bad backwoods slasher movies." +
                " \"Friday The 13th\" and \"The Burning\" may have been the best ones but there were like always a couple of stinkers not far behind like \"Don't Go Into The Woods Alone\" and this one. " +
                "But in all fairness \"The Prey\" is nowhere near as bad as \"Don't Go Into The Woods\" but it's still not great either." +
                " One thing is that it's just boring and acting isn't very good but much better than \"DGITW\" and this movie actually has some attractive looking females to look at, all three of the female leads were stunning." +
                " One thing what is up with all that pointless wildlife footage it just seemed pointless and it looked as the director used that to just used that to fill up some time space." +
                "<br /><br />So, what was there to like about this movie? " +
                "Well, there were a few laugh out loud cheese moments- I couldn't contain a fit of giggles when the final girl did a bizarre type of backwards moon-walk to get away from the kille and there were a few good kill scenes- my favourites being the girl suffocated to death with the sleeping bag; and the phoney looking." +
                "<br /><br />All in all The Prey is dumb, boring and the killer I didn't find scary at all, this movie could have been a whole lot better.";
            var result = await splitter.Process(new ParseRequest(txt)).ConfigureAwait(false);
            var data = ActualWordsHandler.InstanceOpen.Container.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();
            Assert.AreEqual(6, data.Sentences.Count);
            Assert.AreEqual(41, data.Sentences[1].Occurrences.Count());
        }
    }
}
