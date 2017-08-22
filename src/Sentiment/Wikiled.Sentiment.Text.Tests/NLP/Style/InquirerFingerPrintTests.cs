using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP.Style;
using Wikiled.Text.Inquirer.Data;

namespace Wikiled.Sentiment.Text.Tests.NLP.Style
{
    [TestFixture]
    public class InquirerFingerPrintTests
    {
        [Test]
        public async Task GetDataFirst()
        {
            var document = await ActualWordsHandler.Instance.Loader.InitDocument().ConfigureAwait(false);
            var block = new TextBlock(ActualWordsHandler.Instance.WordsHandler, document.Sentences.ToArray());
            Assert.AreEqual(178, block.InquirerFinger.InquirerProbabilities.AllLeafs.Count());
            Assert.AreEqual("PLACE", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(1).First().Name);
            Assert.AreEqual(0.0, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.First().Value, 2));
            Assert.AreEqual("Hostile", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(36).First().Name);
            Assert.AreEqual(0.08, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(35).First().Value, 2));
            Assert.AreEqual("Exch", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(24).First().Name);
            Assert.AreEqual(0.0, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(24).First().Value, 2));
            Assert.AreEqual("Strong", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(34).First().Name);
            Assert.AreEqual(0.17, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(34).First().Value, 2));
            Assert.AreEqual(2, block.InquirerFinger.InquirerProbabilities.Branches.Count);
            Assert.AreEqual(17, block.InquirerFinger.InquirerProbabilities.Branches[0].Branches.Count);
            Assert.AreEqual("Location", block.InquirerFinger.InquirerProbabilities.Branches[0].Branches[0].Name);
            Assert.AreEqual(0.02, Math.Round((double)block.InquirerFinger.InquirerProbabilities.Branches[0].Branches[0].Leafs[0].Value, 2));
        }

        [Test]
        public async Task GetDataSecond()
        {
            var document = await ActualWordsHandler.Instance.Loader.InitDocument("cv001_19502.txt").ConfigureAwait(false);
            var block = new TextBlock(ActualWordsHandler.Instance.WordsHandler, document.Sentences.ToArray());
            Assert.AreEqual(178, block.InquirerFinger.InquirerProbabilities.AllLeafs.Count());
            Assert.AreEqual("PLACE", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(1).First().Name);
            Assert.AreEqual(0.0, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.First().Value, 2));
            Assert.AreEqual("Hostile", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(36).First().Name);
            Assert.AreEqual(0.07, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(35).First().Value, 2));
            Assert.AreEqual("Exch", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(24).First().Name);
            Assert.AreEqual(0.0, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(24).First().Value, 2));
            Assert.AreEqual("Strong", block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(34).First().Name);
            Assert.AreEqual(0.17, Math.Round((double)block.InquirerFinger.InquirerProbabilities.AllLeafs.Skip(34).First().Value, 2));
            Assert.AreEqual(2, block.InquirerFinger.InquirerProbabilities.Branches.Count);
            Assert.AreEqual(17, block.InquirerFinger.InquirerProbabilities.Branches[0].Branches.Count);
            Assert.AreEqual("Location", block.InquirerFinger.InquirerProbabilities.Branches[0].Branches[0].Name);
            Assert.AreEqual(0.02, Math.Round((double)block.InquirerFinger.InquirerProbabilities.Branches[0].Branches[0].Leafs[0].Value, 2));
        }
    }
}
