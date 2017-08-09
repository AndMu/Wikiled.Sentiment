using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP.NRC;

namespace Wikiled.Sentiment.Text.Tests.NLP.NRC
{
    [TestFixture]
    public class NRCDictionaryTests
    {
        [Test]
        public void Test()
        {
            var record = NRCDictionary.Instance.FindRecord(new TestWordItem {Text = "smut"});
            Assert.IsFalse(record.IsAnger);
            Assert.IsFalse(record.IsAnticipation);
            Assert.IsTrue(record.IsDisgust);
            Assert.IsTrue(record.IsFear);
            Assert.IsFalse(record.IsJoy);
            Assert.IsTrue(record.IsNegative);
            Assert.IsFalse(record.IsPositive);
            Assert.IsFalse(record.IsSadness);
            Assert.IsFalse(record.IsSurprise);
            Assert.IsFalse(record.IsTrust);

            record = NRCDictionary.Instance.FindRecord(new TestWordItem { Text = "kill" });
            Assert.IsFalse(record.IsAnger);
            Assert.IsFalse(record.IsAnticipation);
            Assert.IsFalse(record.IsDisgust);
            Assert.IsTrue(record.IsFear);
            Assert.IsFalse(record.IsJoy);
            Assert.IsTrue(record.IsNegative);
            Assert.IsFalse(record.IsPositive);
            Assert.IsTrue(record.IsSadness);
            Assert.IsFalse(record.IsSurprise);
            Assert.IsFalse(record.IsTrust);

            record = NRCDictionary.Instance.FindRecord(new TestWordItem { Text = "xxx", Stemmed = "xxx"});
            Assert.IsNull(record);
        }

        [Test]
        public void TestInverted()
        {
            var record = NRCDictionary.Instance.FindRecord(
                new TestWordItem
                    {
                        Text = "smut",
                        Relationship = new TestWordItemRelationship
                                           {
                                               Inverted = new TestWordItem()
                                           }
                    });
            Assert.IsTrue(record.IsAnger);
            Assert.IsFalse(record.IsAnticipation);
            Assert.IsFalse(record.IsDisgust);
            Assert.IsFalse(record.IsFear);
            Assert.IsFalse(record.IsJoy);
            Assert.IsFalse(record.IsNegative);
            Assert.IsTrue(record.IsPositive);
            Assert.IsFalse(record.IsSadness);
            Assert.IsFalse(record.IsSurprise);
            Assert.IsTrue(record.IsTrust);
        }
    }
}
