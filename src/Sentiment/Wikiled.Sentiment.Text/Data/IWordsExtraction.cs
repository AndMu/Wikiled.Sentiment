using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data
{
    public interface IWordsExtraction
    {
        Document GetDocument(string text);

        Document GetDocumentBySentences(string text, params ISentence[] sentences);
    }
}