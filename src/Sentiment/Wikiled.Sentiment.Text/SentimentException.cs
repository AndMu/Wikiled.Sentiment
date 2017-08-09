using System;
using System.Runtime.Serialization;

namespace Wikiled.Sentiment.Text
{
    [Serializable]
    public class SentimentException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SentimentException()
        {
        }

        public SentimentException(string message) : base(message)
        {
        }

        public SentimentException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SentimentException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
