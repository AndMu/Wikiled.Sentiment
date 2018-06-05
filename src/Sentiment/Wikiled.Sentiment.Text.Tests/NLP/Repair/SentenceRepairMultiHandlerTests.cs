using Moq;
using Wikiled.Sentiment.Text.NLP.Repair;
using NUnit.Framework;

namespace Wikiled.Sentiment.Text.Tests.NLP.Repair
{
    [TestFixture]
    public class SentenceRepairMultiHandlerTests
    {
        [Test]
        public void Repair()
        {
            var repairHandler1 = new Mock<ISentenceRepairHandler>();
            var repairHandler2 = new Mock<ISentenceRepairHandler>();
            repairHandler1.Setup(item => item.Repair("Test")).Returns("Test1");
            repairHandler2.Setup(item => item.Repair("Test1")).Returns("Test2");
            
            SentenceRepairMultiHandler handler = new SentenceRepairMultiHandler();
            handler.Add(repairHandler1.Object);
            handler.Add(repairHandler2.Object);
            var result = handler.Repair("Test");
            Assert.AreEqual("Test2", result);
        }
    }
}
