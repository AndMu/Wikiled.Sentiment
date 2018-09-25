using System;
using System.Linq;
using NUnit.Framework;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Analysis.Arff;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Tests.Arff
{
    [TestFixture]
    public class ArffExtensionsTests
    {
        [Test]
        public void SplitDate()
        {
            var dataSet = ArffDataSet.Create<PositivityType>("Test");
            dataSet.Header.RegisterDate(Constants.DATE);
            dataSet.AddDocument().AddRecord(Constants.DATE).Value = new DateTime(1978, 02, 23);
            dataSet.SplitDate();
            var doc = dataSet.Documents.First();
            var x = (double)doc[Constants.DATE_X].Value;
            var y = (double)doc[Constants.DATE_Y].Value;
            Assert.AreEqual(0.8, Math.Round(x, 2));
            Assert.AreEqual(0.6, Math.Round(y, 2));
        }
    }
}