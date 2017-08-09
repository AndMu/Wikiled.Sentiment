using System;
using System.Xml.Serialization;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Text.Persitency;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Parser.Cache
{
    [XmlRoot("CacheRecord")]
    public class DocumentPersistencyItem : BaseWrappedPersistencyItem<Document, CacheHeaderItem>
    {
        private const string EndTag = "__End__";

        private const string LenTag = "__Len__";

        public DocumentPersistencyItem()
        {
        }

        public DocumentPersistencyItem(Document document)
            : base(DateTime.UtcNow, DateTime.UtcNow.ToShortDateString().CreatePureLetterText(), "document", document)
        {
            int total = Instance.Text.Length < 10 ? Instance.Text.Length : 10;
            Beggining = Instance.Text.Substring(0, total).CreatePureLetterText();
            Ending = Instance.Text.Substring(Instance.Text.Length - total, total).CreatePureLetterText();
            Length = Instance.Text.Length;
            Date = Date;
            Tag = string.Format(
                "{0}{3}{1}{4}{2}",
                Beggining,
                Ending,
                Length,
                EndTag,
                LenTag);
        }

        public string Beggining { get; }

        public string Ending { get; }

        public int Length { get; }

        public override CacheHeaderItem GenerateIndex(string file)
        {
            CacheHeaderItem searchItem = new CacheHeaderItem();
            searchItem.Beggining = Beggining;
            searchItem.Ending = Ending;
            searchItem.Length = Length;
            searchItem.File = file;
            searchItem.Tag = Tag;
            searchItem.Date = Date;
            return searchItem;
        }
    }
}
