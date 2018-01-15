using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.AcceptanceTests.Resources
{
    [TestFixture]
    public class DataDownloaderTests
    {
        private DataDownloader instance;

        private string output;

        [SetUp]
        public void SetUp()
        {
            output = Path.Combine(TestContext.CurrentContext.TestDirectory, "out");
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }

            instance = new DataDownloader();
        }

        [Test]
        public async Task TestCleanup()
        {
            await instance.DownloadFile(new Uri("http://datasets.azurewebsites.net/Resources/test.zip"), output);
            var result = Directory.Exists(output);
            Assert.IsTrue(result);
            var files = Directory.GetFiles(output);
            Assert.AreEqual(1, files.Length);
        }
    }
}