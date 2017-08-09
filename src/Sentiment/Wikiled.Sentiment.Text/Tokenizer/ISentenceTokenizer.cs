using System.Collections.Generic;
using Wikiled.Text.Analysis.Tokenizer;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public interface ISentenceTokenizer
    {
        IEnumerable<IWordsTokenizer> Parse(string text);

        IEnumerable<string> Split(string text);

        IWordsTokenizerFactory TokenizerFactory { get; }
    }
}