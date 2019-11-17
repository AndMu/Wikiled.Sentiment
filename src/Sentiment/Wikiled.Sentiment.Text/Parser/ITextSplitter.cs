using System;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Light;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ITextSplitter : IDisposable
    {
        Task<LightDocument> Process(ParseRequest request);
    }
}