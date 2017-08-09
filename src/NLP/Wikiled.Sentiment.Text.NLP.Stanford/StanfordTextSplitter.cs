using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    /// <summary>
    /// This class is limited by internal threadpool. You can't parralerize it further as it consumes a lot of memory
    /// </summary>
    public class StanfordTextSplitter : BaseTextSplitter
    {
        private readonly StanfordNLPProxy detector;

        public StanfordTextSplitter(string resourcePath, IWordsHandler handler, ICachedDocumentsSource cache)
            : base(handler, cache)
        {
            if (detector == null)
            {
                detector = new StanfordNLPProxy(resourcePath, handler);
            }
        }
        
        protected override Document ActualProcess(ParseRequest request)
        {
            return detector.Process(request);
        }
    }
}