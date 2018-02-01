using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Wikiled.Core.Utility.Resources;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Features;

namespace Wikiled.Sentiment.Text.Tests.Features
{
    [TestFixture]
    public class DetectionDataTests
    {
        [Test]
        public void Deserialise()
        {
            XDocument document = typeof(DetectionData).LoadXmlData("Resources.Features.Features.xml");
            var data = document.XmlDeserialize<DetectionData>("features");
            Assert.AreEqual(17, data.Adjectives.Length);
            DetectionBlock block = (from item in data.Adjectives where item.Name == "Senses" select item).FirstOrDefault();
            Assert.IsNotNull(block);
            Assert.AreEqual("Senses", block.Name);
            Assert.AreEqual(122, block.Words.Length);
        }
    }
}
