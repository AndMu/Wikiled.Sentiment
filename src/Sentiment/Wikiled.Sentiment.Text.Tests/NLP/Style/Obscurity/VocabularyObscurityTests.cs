using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.NLP.Style;

namespace Wikiled.Sentiment.Text.Tests.NLP.Style.Obscurity
{
    [TestFixture]
    public class VocabularyObscurityTests
    {
        [Test]
        public async Task GetDataFirst()
        {
            var document = await ActualWordsHandler.Instance.Loader.InitDocument().ConfigureAwait(false);
            TextBlock block = new TextBlock(ActualWordsHandler.Instance.WordsHandler, document.Sentences.ToArray());
            Assert.Greater(block.VocabularyObscurity.BNC.Top100Words, 0);
        }
    }
}
