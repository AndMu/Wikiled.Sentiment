using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public interface IPipeline<T>
    {
        IEnumerable<T> Process(IEnumerable<T> words);
    }
}
