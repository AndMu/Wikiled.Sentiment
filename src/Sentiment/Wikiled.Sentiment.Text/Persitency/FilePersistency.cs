using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Serialization;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class FilePersistency<T> : IFilePersistency<T>
        where T : class
    {
        public event EventHandler<PersitencyEventArgs<T>> Saved;

        public event EventHandler<PersitencyEventArgs<T>> Loaded;

        public event EventHandler<LoadingEventArgs> Loading;

        private readonly object syncRoot = new Object();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public FilePersistency(bool commpressed)
        {
            UseCompression = commpressed;
        }

        public IEnumerable<T> LoadAll(DirectoryInfo directory)
        {
            return directory.GetFiles().Select(LoadItem);
        }

        public T LoadItem(FileInfo file)
        {
            try
            {
                var loading = Loading;
                if (loading != null)
                {
                    var args = new LoadingEventArgs(file);
                    loading(this, args);
                    if (args.Cancel)
                    {
                        return null;
                    }
                }

                var item = file.XmlDeserialize<T>(Encoding.UTF8, "DataItem");
                Loaded?.Invoke(this, new PersitencyEventArgs<T>(file, item));
                return item;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return null;
        }

        public FileInfo Save(T record, string name)
        {
            string file = string.Format("{0}.{1}", name, UseCompression ? "zip" : "xml");
            var folder = Path.GetDirectoryName(name);
            lock (syncRoot)
            {
                folder.EnsureDirectoryExistence();
            }

            var fileInfo = new FileInfo(file);
            fileInfo.XmlSerialize(record, UseCompression);
            if (Saved != null)
            {
                Saved(this, new PersitencyEventArgs<T>(fileInfo, record));
            }

            return fileInfo;
        }

        public bool UseCompression { get; private set; }
    }
}
