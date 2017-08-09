using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Async;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class IndexedPersistency<T, TI> : IIndexedPersistency<T, TI>
        where TI : class, IIndexRegistry
        where T : class, IPersistencyItem<TI>
    {
        private const string CacheNamespace = "Data";

        private readonly FileInfo headerFile;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly AsynThrottledAction saveAction = new AsynThrottledAction(TimeSpan.FromSeconds(10));

        private readonly object syncRoot = new object();

        protected IndexRegistryHandler<TI> Handler;

        public IndexedPersistency(string name, string path)
            : this(new MultiThroughFilePersistencyHandler<T, TI>(name, path))
        {
        }

        public IndexedPersistency(string path)
            : this(new MultiThroughFilePersistencyHandler<T, TI>(path))
        {
        }

        private IndexedPersistency(MultiThroughFilePersistencyHandler<T, TI> persistencyHandler)
        {
            PersistencyHandler = persistencyHandler;
            PersistencyHandler.Persistency.Saved += PersistencyOnSaved;
            headerFile = new FileInfo(Path.Combine(persistencyHandler.PersistencyPath.FullName, "header.xml"));
            UseHeader = true;
            Handler = new IndexRegistryHandler<TI>();
        }

        public MultiThroughFilePersistencyHandler<T, TI> PersistencyHandler { get; set; }

        public TI[] Records => Handler.Records;

        public bool UseHeader { get; }

        public void Dispose()
        {
            saveAction.Dispose();
        }

        public IEnumerable<Lazy<T>> GetAllItems()
        {
            return GetItemsInternal(() => Handler.Records);
        }

        public IEnumerable<Lazy<T>> GetItems(string query)
        {
            Guard.NotNullOrEmpty(() => query, query);
            return GetItemsInternal(() => Handler.Find(query));
        }

        public IEnumerable<Lazy<T>> GetItemsByDate(DateTime date)
        {
            return GetItemsInternal(() => Handler.FindByDate(date));
        }

        public void LoadHeader()
        {
            try
            {
                if(!UseHeader)
                {
                    return;
                }

                lock(syncRoot)
                {
                    Handler = headerFile.XmlDeserialize<IndexRegistryHandler<TI>>(Encoding.UTF8, CacheNamespace);
                }
            }
            catch(Exception ex)
            {
                log.Warn(ex.ToString());
                RebuildIndex();
            }
        }

        public void RebuildIndex()
        {
            if(!UseHeader)
            {
                return;
            }

            PersistencyHandler.Persistency.Loaded += PersistencyOnSaved;
            PersistencyHandler.Load();
            PersistencyHandler.Persistency.Loaded -= PersistencyOnSaved;
        }

        public void Save(T item)
        {
            Guard.NotNull(() => item, item);
            PersistencyHandler.Add(item);
        }

        public void Save()
        {
            saveAction.SaveAll(() => Handler.XmlSerialize(CacheNamespace).SaveSafe(headerFile.FullName));
        }

        protected virtual IEnumerable<TI> OnPersistencySaved(string file, T record)
        {
            yield return record.GenerateIndex(file);
        }

        protected IEnumerable<Lazy<T>> GetItemsInternal(Func<IEnumerable<TI>> source)
        {
            foreach(var registry in source().Select(item => item.File).Distinct())
            {
                FileInfo file = new FileInfo(Path.Combine(PersistencyHandler.PersistencyPath.FullName, registry));
                if(!file.Exists)
                {
                    log.Debug("File not found: {0}", file.FullName);
                    continue;
                }

                yield return new Lazy<T>(() => PersistencyHandler.Persistency.LoadItem(file));
            }
        }

        private void PersistencyOnSaved(object sender, PersitencyEventArgs<T> persitencyEventArgs)
        {
            if(!UseHeader)
            {
                return;
            }

            string relativeFile = (PersistencyHandler.PersistencyPath.FullName + '\\').GetRelativePath(persitencyEventArgs.File.FullName);
            foreach(var item in OnPersistencySaved(relativeFile, persitencyEventArgs.Record))
            {
                Handler.Add(item);
                Save();
            }
        }
    }
}
