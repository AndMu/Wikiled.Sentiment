using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class SingleFilePersistencyHandler<T, TI> : PersistencyHandlerBase<T, TI>
        where T : class, IPersistencyItem<TI>
        where TI : class, IIndexRegistry
    {
        private readonly IFilePersistency<List<T>> persistency;

        public SingleFilePersistencyHandler(string name, string path, bool compress = false)
            : base(name, path)
        {
            persistency = new FilePersistency<List<T>>(compress);
        }

        public SingleFilePersistencyHandler(string path, bool compress = false)
            : base(path)
        {
            persistency = new FilePersistency<List<T>>(compress);
        }

        protected override IEnumerable<T> LoadRecords()
        {
            return persistency.LoadAll(PersistencyPath).SelectMany(lists => lists);
        }

        protected override void Save(T record)
        {
        }

        protected override void Save(string index, List<T> list)
        {
            index = index.CreatePureLetterText();
            string fileName = Path.Combine(PersistencyPath.FullName, index);
            persistency.Save(list, fileName);
        }
    }
}
