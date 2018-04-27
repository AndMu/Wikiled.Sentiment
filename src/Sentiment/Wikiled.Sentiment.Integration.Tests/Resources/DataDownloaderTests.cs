using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Integration.Tests.Resources
{
    [TestFixture]
    public class DataDownloaderTests
    {
        [Test]
        public async Task Construct()
        {
            var output = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestOut");
            if (Directory.Exists(output))
            {
                Directory.Delete(output);
            }

            await new DataDownloader().DownloadFile(new Uri("http://datasets.azurewebsites.net/Resources/Test.zip"), output).ConfigureAwait(false);
            Assert.True(Directory.Exists(output));
            Assert.AreEqual(1, Directory.GetFiles(output).Length);
        }
    }
}