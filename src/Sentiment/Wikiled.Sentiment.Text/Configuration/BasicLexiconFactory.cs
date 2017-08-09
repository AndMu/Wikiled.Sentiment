using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class BasicLexiconFactory : ILexiconFactory
    {
        public bool CanConstruct => !IsConstructed;

        public bool IsConstructed { get; private set; }

        public IWordsHandler WordsHandler { get; private set; }

        public void Construct()
        {
            WordsHandler = new BasicWordsHandler(new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance));
            IsConstructed = true;
        }
    }
}