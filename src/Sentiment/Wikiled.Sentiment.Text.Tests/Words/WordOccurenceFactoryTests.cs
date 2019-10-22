using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Common.Testing.Utilities.Reflection;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Tests.Words
{
    [TestFixture]
    public class WordOccurenceFactoryTests
    {
        private WordsHandlerHelper helper;

        private WordOccurenceFactory instance;

        [SetUp]
        public void Setup()
        {
            helper = new WordsHandlerHelper();

            instance = new WordOccurenceFactory(new NullLogger<WordOccurenceFactory>(),
                                                helper.Handler.Object,
                                                helper.RawTextExractor.Object,
                                                helper.InquirerManager.Object);
        }

        [Test]
        public void Create()
        {
            ConstructorHelper.ConstructorMustThrowArgumentNullException<WordOccurenceFactory>();
            Assert.IsNotNull(instance);
        }

        [Test]
        public void CreateWord()
        {
            var word = instance.CreateWord("Test", POSTags.Instance.NN);
            Assert.IsNotNull(word);
            word = instance.CreateWord("Test", "NN");
            Assert.IsNotNull(word);
        }

        [Test]
        public void CreatePhrase()
        {
            var word = instance.CreatePhrase("NN");
            Assert.IsNotNull(word);
        }
    }
}
