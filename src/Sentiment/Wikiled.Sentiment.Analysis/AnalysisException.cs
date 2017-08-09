using System;
using System.Runtime.Serialization;

namespace Wikiled.Sentiment.Analysis
{
    [Serializable]
    public class AnalysisException : Exception
    {
        public AnalysisException()
        {
        }

        public AnalysisException(string message) : base(message)
        {
        }

        public AnalysisException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AnalysisException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
