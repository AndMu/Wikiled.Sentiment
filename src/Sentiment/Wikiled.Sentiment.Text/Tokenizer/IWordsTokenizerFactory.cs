using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public interface IWordsTokenizerFactory
    {
        IWordsTokenizer Create(string sentence);

        CombinedPipeline<WordEx> WordItemPipeline { get; }

        CombinedPipeline<string> Pipeline { get; }
    }
}
