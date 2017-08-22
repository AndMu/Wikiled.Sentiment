using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Text.Tests.NLP
{
    [TestFixture]
    public class NRCDictionaryExtensionTests
    {
        [Test]
        public void TestInverted()
        {
            NRCDictionary dictionary = new NRCDictionary();
            {
                var record = dictionary.FindRecord(
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
}
