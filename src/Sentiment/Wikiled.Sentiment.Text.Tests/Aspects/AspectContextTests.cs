﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Tests.Aspects
{
    [TestFixture]
    public class AspectContextTests
    {
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AspectContext(true, null));
        }

        [Test]
        public async Task Process()
        {
            var data = await ActualWordsHandler.InstanceSimple.TextSplitter.Process(new ParseRequest("I like my school teacher.")).ConfigureAwait(false);
            var document = data.Construct(ActualWordsHandler.InstanceSimple.WordFactory);
            var review = ActualWordsHandler.InstanceSimple.Container.Resolve<Func<Document, IParsedReviewManager>>()(document).Create();
            var context = new AspectContext(true, review.ImportantWords.ToArray());
            context.Process();
            var attributes = context.GetAttributes().ToArray();
            var features = context.GetFeatures().ToArray();
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual("like", attributes[0].Text);
            Assert.AreEqual(2, features.Length);
            Assert.IsTrue(features.Any(item => item.Text == "school"));
            Assert.IsTrue(features.Any(item => item.Text == "teacher"));
        }
    }
}
