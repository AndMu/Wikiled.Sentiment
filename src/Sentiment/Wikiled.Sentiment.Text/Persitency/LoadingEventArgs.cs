using System.IO;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class LoadingEventArgs
    {
        public LoadingEventArgs(FileInfo info)
        {
            Info = info;
        }

        public FileInfo Info { get; private set; }

        public bool Cancel { get; set; }
    }
}
