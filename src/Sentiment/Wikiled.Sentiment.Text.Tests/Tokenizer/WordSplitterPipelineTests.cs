using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wikiled.Sentiment.Text.Tokenizer;

namespace Wikiled.Sentiment.Text.Tests.Tokenizer
{
    [TestClass]
    public class WordSplitterPipelineTests
    {
        [TestMethod]
        public void Process()
        {
            string[] result = WordSplitterPipeline.Instance
                .Process(new[] {"utter-cool", "way"})
                .ToArray();
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("utter", result[0]);
            Assert.AreEqual("cool", result[1]);
            Assert.AreEqual("way", result[2]);
        }
    }
}
