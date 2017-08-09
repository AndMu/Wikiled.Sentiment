using NUnit.Framework;
using Wikiled.Sentiment.Text.Helpers;

namespace Wikiled.Sentiment.Text.Tests.Helpers
{
    [TestFixture]
    public class CalculatorTests
    {
        [Test]
        public void Add()
        {
            object value1 = 1;
            object value2 = 2;
            var resutl = Calculator<object>.Add(value1, value2);
        }
    }
}
