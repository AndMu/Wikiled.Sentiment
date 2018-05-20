using System;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.NLP
{
    public interface ISplitterFactory : IConfigurationFactory, IDisposable
    {
        ITextSplitter ConstructSingle();
    }
}