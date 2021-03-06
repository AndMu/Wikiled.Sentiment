﻿using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP.Repair;

namespace Wikiled.Sentiment.Integration.Tests.NLP
{
    [TestFixture]
    public class SentenceRepairHandlerTests
    {
        private IContextSentenceRepairHandler handler;

        private ISentenceRepairHandler repairHandler;

        [OneTimeSetUp]
        public void Setup()
        {
            handler = ActualWordsHandler.InstanceSimple.Container.Resolve<IContextSentenceRepairHandler>();
            repairHandler = ActualWordsHandler.InstanceSimple.Container.Resolve<ISentenceRepairHandler>();
        }

        [Test]
        public void RepairNotOnly()
        {
            for (var i = 0; i < 1000; i++)
            {
                var result = repairHandler.Repair(handler.Repair("We not only brought this book but also liked it"));
                Assert.AreEqual("We brought this book and liked it", result);
            }
        }

        [Test]
        public void Alot()
        {
            var result = repairHandler.Repair(handler.Repair("bottom line camera give a lot of bang for buck"));
            Assert.AreEqual("bottom line camera give alot bang for buck", result);
        }

        [Test]
        public void RepairEmoticons()
        {
            var result = repairHandler.Repair(handler.Repair("It was :) but I :("));
            Assert.AreEqual("It was EMOTICON_slightly_smiling_face but I EMOTICON_disappointed", result);
        }

        [Test]
        public void RepairSlang()
        {
            var result = repairHandler.Repair(handler.Repair("It was afaik"));
            Assert.AreEqual("It was as far as I know", result);
        }

        [Test]
        public void Enough()
        {
            var result = repairHandler.Repair(handler.Repair("It is heavy enough to make it stable, but not too heavy to preclude hand use."));
            Assert.AreEqual("It is heavy enough to make it stable, but not too heavy to preclude hand use.", result);
        }

        [Test]
        public void Unison()
        {
            var result = repairHandler.Repair(handler.Repair("i think this girl unison is nice"));
            Assert.AreEqual("i think this girl unison is nice", result);
        }

        [Test]
        public void Non()
        {
            var result = repairHandler.Repair(handler.Repair("i think non-worthy this girl nonworthy is nice"));
            Assert.AreEqual("i think not worthy this girl not worthy is nice", result);
        }
    }
}
