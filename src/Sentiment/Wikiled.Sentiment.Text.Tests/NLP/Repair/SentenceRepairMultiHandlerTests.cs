using Wikiled.Sentiment.Text.NLP.Repair;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wikiled.Sentiment.Text.Tests.NLP.Repair
{
    [TestFixture]
    public class SentenceRepairMultiHandlerTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Repair()
        {
            var repairHandler1 = mocks.StrictMock<ISentenceRepairHandler>();
            var repairHandler2 = mocks.StrictMock<ISentenceRepairHandler>();
            Expect.Call(repairHandler1.Repair("Test")).Return("Test1");
            Expect.Call(repairHandler2.Repair("Test1")).Return("Test2");
            mocks.ReplayAll();
            SentenceRepairMultiHandler handler = new SentenceRepairMultiHandler();
            handler.Add(repairHandler1);
            handler.Add(repairHandler2);
            var result = handler.Repair("Test");
            Assert.AreEqual("Test2", result);
            mocks.VerifyAll();
        }
    }
}
