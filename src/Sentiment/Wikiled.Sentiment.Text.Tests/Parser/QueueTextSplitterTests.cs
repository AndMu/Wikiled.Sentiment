﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Tests.Parser
{
    [TestFixture]
    public class QueueTextSplitterTests
    {
        private Mock<ITextSplitter> splitter;

        private QueueTextSplitter instance;

        [SetUp]
        public void Setup()
        {
            splitter = new Mock<ITextSplitter>();
            instance = new QueueTextSplitter(new NullLogger<QueueTextSplitter>(), 3, () => splitter.Object);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new QueueTextSplitter(new NullLogger<QueueTextSplitter>(), 5, null));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new QueueTextSplitter(new NullLogger<QueueTextSplitter>(), 0, () => splitter.Object));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(4, 3)]
        [TestCase(10, 3)]
        public async Task Process(int times, int construction)
        {
            var request = new ParseRequest("Test");
            splitter.Setup(item => item.Process(request))
                    .Returns(
                        async () =>
                        {
                            await Task.Delay(50).ConfigureAwait(false); 
                            return null;
                        });
                
            var tasks = new List<Task>();
            for(var i = 0; i < times; i++)
            {
                tasks.Add(instance.Process(request));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            instance.Dispose();
            splitter.Verify(item => item.Process(request), Times.Exactly(times));
            splitter.Verify(item => item.Dispose(), Times.Exactly(construction));
        }
    }
}
