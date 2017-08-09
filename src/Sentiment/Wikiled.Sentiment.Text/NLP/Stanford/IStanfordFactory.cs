using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    public interface IStanfordFactory
    {
        ISplitterFactory Create(ILexiconFactory lexiconFactory, IConfigurationHandler configuration);
    }
}