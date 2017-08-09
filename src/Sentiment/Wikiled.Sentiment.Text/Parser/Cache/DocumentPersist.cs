using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Persitency;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser.Cache
{
    public class DocumentPersist : IndexedPersistency<DocumentPersistencyItem, CacheHeaderItem>, ICachedDocumentsSource
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static DocumentPersist CreateStandard(string path)
        {
            var instance = new DocumentPersist("Documents", path);
            instance.LoadHeader();
            return instance;
        }

        private DocumentPersist(string name, string path)
            : base(name, path)
        {
        }

        public Task<bool> Save(Document document)
        {
            Guard.NotNull(() => document, document);
            try
            {
                Save(new DocumentPersistencyItem(document));
                return Task.FromResult(true);
            }
            catch (Exception exception)
            {
                log.Error(exception);
            }

            return Task.FromResult(false);
        }

        public Task<Document> GetCached(Document original)
        {
            return Task.FromResult((Document)null);
        }

        public Task<Document> GetCached(string text)
        {
            Guard.NotNullOrEmpty(() => text, text);
            return Task.FromResult(GetDocuments(text).FirstOrDefault());
        }

        public IEnumerable<Document> GetAllDocuments()
        {
            return GetAllItems().Select(item => item.Value.Instance);
        }

        public IEnumerable<Document> GetDocuments(string text)
        {
            var document = new Document(text);
            return GetItems(new DocumentPersistencyItem(document).Tag).Select(item => item.Value.Instance);
        }

        public Task<Document> GetById(string id)
        {
            throw new NotSupportedException();
        }
    }
}
