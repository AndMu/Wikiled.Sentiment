using Wikiled.Text.Analysis.WordNet.Engine;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.WordNet.InformationContent
{
    public interface IRelatednessMesaure
    {
        double Measure(SynSet synSet1, SynSet synSet2);

        double Measure(string word1, string word2, WordType type = WordType.Noun);

        double Measure(IWordItem first, IWordItem second, WordType enforceType = WordType.Unknown);
    }
}