using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Sentiment.Text.Persitency
{
    public abstract class PersistencyHandlerBase<T, TI> : IDisposable
        where T : class, IPersistencyItem<TI>
        where TI : class, IIndexRegistry
    {
        private readonly Dictionary<string, List<T>> indexes = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, T> indexTable = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, List<T>> tagTable = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);

        private readonly object syncRoot = new object();

        protected PersistencyHandlerBase(string name, string path)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Guard.NotNullOrEmpty(() => path, path);
            PersistencyPath = new DirectoryInfo(Path.Combine(path, name));
            Name = name;
            PersistencyPath.EnsureDirectoryExistence();
        }

        protected PersistencyHandlerBase(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            PersistencyPath = new DirectoryInfo(path);
            Name = Path.GetDirectoryName(path);
            PersistencyPath.EnsureDirectoryExistence();
        }

        public string Name { get; private set; }

        public DirectoryInfo PersistencyPath { get; }

        public virtual void Add(params T[] records)
        {
            Guard.NotNull(() => records, records);
            log.Debug("Add {0}", records.Length);
            if (records.Length == 0)
            {
                return;
            }

            bool added = false;
            foreach (var record in records)
            {
                if (!indexTable.ContainsKey(GetTag(record.Type, record.Date)) ||
                    !tagTable.ContainsKey(record.Tag))
                {
                    RegisterRecord(record);
                    Save(record);
                    added = true;
                }
            }

            if (added)
            {
                List<T> list;
                lock (syncRoot)
                {
                    list = indexes.GetSafeCreate(records[0].Type);
                }

                Save(records[0].Type, list);
            }
        }

        public virtual void Dispose()
        {
        }

        public virtual List<T> RetrieveAll(string index)
        {
            log.Debug("RetrieveAll: {0}", index);
            lock (syncRoot)
            {
                List<T> list;
                if (!indexes.TryGetValue(index, out list))
                {
                    log.Debug("Not found");
                    return new List<T>();
                }

                return list.ToList();
            }
        }

        public void Load()
        {
            log.Debug("Load");
            lock (syncRoot)
            {
                indexTable.Clear();
                tagTable.Clear();
                indexes.Clear();
            }

            Parallel.ForEach(
                LoadRecords(),
                new ParallelOptions {MaxDegreeOfParallelism = 8},
                item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    RegisterRecord(item);
                });
        }

        public T[] Retrieve(string index, DateTime date, int max)
        {
            log.Debug("Retrieve: {0}:{1}", index, date);
            var list = RetrieveItems(index, date, max);
            if (list == null)
            {
                return new T[] {};
            }

            return list.Where(item => item.Date <= date)
                       .OrderByDescending(item => item.Date)
                       .Take(max)
                       .ToArray();
        }

        protected abstract IEnumerable<T> LoadRecords();

        protected abstract void Save(T record);

        protected abstract void Save(string index, List<T> list);

        protected virtual void RegisterRecord(T record)
        {
            lock (syncRoot)
            {
                indexTable[GetTag(record.Type, record.Date)] = record;
                tagTable.GetSafeCreate(record.Tag).Add(record);
                indexes.GetSafeCreate(record.Type).Add(record);
            }
        }

        protected virtual List<T> RetrieveItems(string index, DateTime date, int max)
        {
            log.Debug("Retrieve: {0}:{1}", index, date);
            lock (syncRoot)
            {
                List<T> list;
                if (!indexTable.ContainsKey(GetTag(index, date)) ||
                    !indexes.TryGetValue(index, out list))
                {
                    log.Debug("Not found");
                    return null;
                }

                return list;
            }
        }

        private string GetTag(string index, DateTime date)
        {
            return $"{date.ToShortDateString()}-{index}";
        }
    }
}
