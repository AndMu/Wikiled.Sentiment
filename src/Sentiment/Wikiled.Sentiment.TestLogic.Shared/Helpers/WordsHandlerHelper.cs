using Moq;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
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
            Handler = new Mock<IContextWordsHandler>();
            AspectDectector = new Mock<IAspectDectector>();
            RawTextExractor = new Mock<IRawTextExtractor>();
            RawTextExractor.Setup(item => item.GetWord(It.IsAny<string>())).Returns((string myval) => myval);
            
            InquirerManager = new Mock<IInquirerManager>();
            Dictionary = new Mock<INRCDictionary>();
            Handler.Setup(item => item.Context).Returns(new SentimentContext());
        }

        public Mock<IInquirerManager> InquirerManager { get; }

        public Mock<INRCDictionary> Dictionary { get; }

        public Mock<IContextWordsHandler> Handler { get; }

        public Mock<IAspectDectector> AspectDectector { get; }

        public Mock<IRawTextExtractor> RawTextExractor { get; }
    }
}
