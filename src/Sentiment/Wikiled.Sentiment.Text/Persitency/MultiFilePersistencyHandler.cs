using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Sentiment.Text.Persitency
{
    public class MultiFilePersistencyHandler<T, TI> : PersistencyHandlerBase<T, TI>
        where TI : class, IIndexRegistry
        where T : class, IPersistencyItem<TI>
    {
        public IFilePersistency<T> Persistency { get; private set; }

        public MultiFilePersistencyHandler(string path, bool compress = true)
            : base(path)
        {
            Persistency = new FilePersistency<T>(compress);
        }

        public MultiFilePersistencyHandler(string name, string path, bool compress = true)
            : base(name, path)
        {
            Persistency = new FilePersistency<T>(compress);
        }

        protected override void Save(T record)
        {
            var file = Path.Combine(Path.Combine(PersistencyPath.FullName, record.Type), record.Tag.CreatePureLetterText());
            Persistency.Save(record, file);
        }

        protected override IEnumerable<T> LoadRecords()
        {
            return PersistencyPath.GetDirectories()
                .SelectMany(item => Persistency.LoadAll(item));
        }

        protected override void Save(string index, List<T> list)
        {
        }
    }
}