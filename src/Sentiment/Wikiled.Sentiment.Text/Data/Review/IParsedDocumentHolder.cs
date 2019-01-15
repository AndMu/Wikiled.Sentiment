using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data.Review
{
    public interface IParsedDocumentHolder
    {
        Task<Document> GetParsed();

        Task<Document> GetOriginal();
    }
}
