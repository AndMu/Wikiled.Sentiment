using Moq;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class WordsHandlerHelper
    {
        public WordsHandlerHelper()
        {
            Handler = new Mock<IWordsHandler>();
            AspectDectector = new Mock<IAspectDectector>();
            RawTextExractor = new Mock<IRawTextExtractor>();
            Handler.Setup(item => item.AspectDectector).Returns(AspectDectector.Object);
            Handler.Setup(item => item.Extractor).Returns(RawTextExractor.Object);
            Loader = new Mock<DocumentLoader>();
            RawTextExractor.Setup(item => item.GetWord(It.IsAny<string>())).Returns((string myval) => myval);
            InquirerManager = new Mock<IInquirerManager>();
            Handler.Setup(item => item.InquirerManager).Returns(InquirerManager.Object);

            Dictionary = new Mock<INRCDictionary>();
            Handler.Setup(item => item.NRCDictionary).Returns(Dictionary.Object);
        }

        public Mock<IInquirerManager> InquirerManager { get; }

        public Mock<INRCDictionary> Dictionary { get; }

        public Mock<IWordsHandler> Handler { get; }

        public Mock<IAspectDectector> AspectDectector { get; }

        public Mock<IRawTextExtractor> RawTextExractor { get; }

        public Mock<DocumentLoader> Loader { get; }
    }
}
