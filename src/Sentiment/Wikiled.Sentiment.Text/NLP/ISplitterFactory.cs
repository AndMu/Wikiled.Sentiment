using System;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public interface ISplitterFactory : IDisposable
    {
        ITextSplitter ConstructSingle();
    }
}