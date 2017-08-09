using System;
using System.Runtime.Serialization;

namespace Wikiled.Sentiment.Text.Helpers
{
    [Serializable]
    public class ParsingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ParsingException()
        {
        }

        public ParsingException(string message)
            : base(message)
        {
        }

        public ParsingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ParsingException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
