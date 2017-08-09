using System;
using System.Runtime.Serialization;

namespace Wikiled.Sentiment.Analysis.Workspace
{
    [Serializable]
    public class WorkspaceException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public WorkspaceException()
        {
        }

        public WorkspaceException(string message) : base(message)
        {
        }

        public WorkspaceException(string message, Exception inner) : base(message, inner)
        {
        }

        protected WorkspaceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
