using System;
using System.IO;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class PersitencyEventArgs<T> : EventArgs
    {
        public PersitencyEventArgs(FileInfo file, T record)
        {
            Guard.NotNull(() => file, file);
            File = file;
            Record = record;
        }

        public FileInfo File { get; private set; }

        public T Record { get; private set; }
    }
}
