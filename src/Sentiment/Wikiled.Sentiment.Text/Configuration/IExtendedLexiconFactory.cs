using Wikiled.Text.Analysis.WordNet.Engine;
using Wikiled.Text.Analysis.WordNet.InformationContent;

namespace Wikiled.Sentiment.Text.Configuration
{
    public interface IExtendedLexiconFactory : ILexiconFactory
    {
        IRelatednessMesaure RelatednessMeasure { get; }

        IInformationContentResnik Resnik { get; }

        IWordNetEngine WordNetEngine { get; }

        string WordNetPath { get; }

        string WordNetInfoContentPath { get; }

        string ResourcesPath { get; }
    }
}
