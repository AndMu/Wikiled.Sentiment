using System;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser
{
    public interface ITextSplitter : IDisposable
    {
        Task<Document> Process(ParseRequest request);
    }
}