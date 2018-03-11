using System;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Helpers;

namespace Wikiled.Sentiment.Text.Tests.Helpers
{
    [TestFixture]
    public class AutoEvictingDictionaryTests
    {
        [Test]
        public void TestDefault()
        {
            AutoEvictingDictionary<string, string> dictionary = new AutoEvictingDictionary<string, string>();
            dictionary.Add("Test", "DataValue");
            Assert.AreEqual("DataValue", dictionary.Get("Test"));
            Assert.AreEqual(null, dictionary.Get("test"));
            dictionary.Increment();
            dictionary.Increment();
            dictionary.Increment();
            Assert.AreEqual("DataValue", dictionary.Get("Test"));
            dictionary.Increment();
            Assert.AreEqual(null, dictionary.Get("Test"));
        }

        [Test]
        public void Test()
        {
            AutoEvictingDictionary<string, string> dictionary = new AutoEvictingDictionary<string, string>(StringComparer.OrdinalIgnoreCase, 2);
            dictionary.Add("Test", "DataValue");
            Assert.AreEqual("DataValue", dictionary.Get("Test"));
            Assert.AreEqual("DataValue", dictionary.Get("test"));
            dictionary.Increment();
            Assert.AreEqual("DataValue", dictionary.Get("Test"));
            dictionary.Increment();
            Assert.AreEqual(null, dictionary.Get("Test"));
        }

        [Test]
        public void AddSame()
        {
            AutoEvictingDictionary<int, string> dictionary = new AutoEvictingDictionary<int, string>();
            dictionary[1] = "Test";
            Assert.AreEqual("Test", dictionary[1]);
            dictionary.Increment();
            dictionary.Increment();
            Assert.AreEqual("Test", dictionary[1]);
            dictionary[1] = "Test";
            dictionary.Increment();
            dictionary.Increment();
            Assert.AreEqual("Test", dictionary[1]);
            dictionary.Increment();
            Assert.AreEqual("Test", dictionary[1]);
            dictionary.Increment();
            Assert.AreEqual(null, dictionary[1]);
        }
    }
}
