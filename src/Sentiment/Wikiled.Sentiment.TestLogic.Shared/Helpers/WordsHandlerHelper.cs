using Autofac;
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
            RawTextExractor.Setup(item => item.GetWord(It.IsAny<string>())).Returns((string myval) => myval);
            
            InquirerManager = new Mock<IInquirerManager>();
            Dictionary = new Mock<INRCDictionary>();
        }

        public Mock<IInquirerManager> InquirerManager { get; }

        public Mock<INRCDictionary> Dictionary { get; }

        public Mock<IWordsHandler> Handler { get; }

        public Mock<IAspectDectector> AspectDectector { get; }

        public Mock<IRawTextExtractor> RawTextExractor { get; }
    }
}
