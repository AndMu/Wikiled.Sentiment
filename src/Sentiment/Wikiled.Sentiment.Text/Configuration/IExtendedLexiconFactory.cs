using Wikiled.Sentiment.Text.WordNet.InformationContent;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Text.Configuration
{
    public interface IExtendedLexiconFactory : ILexiconFactory
    {
        IRelatednessMesaure RelatednessMesaure { get; }

        IInformationContentResnik Resnik { get; }

        IWordNetEngine WordNetEngine { get; }

        string WordNetPath { get; }

        string WordNetInfoContentPath { get; }

        string ResourcesPath { get; }
    }
}
