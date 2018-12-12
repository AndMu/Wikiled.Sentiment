using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.Twitter;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class SentenceRepairHandler : ISentenceRepairHandler
    {
        private readonly List<SentenceRepair> repairs = new List<SentenceRepair>();

        private readonly string resourcesPath;

        private readonly IWordsDictionary dictionary;

        private readonly IMessageCleanup cleanup;

        public SentenceRepairHandler(ILexiconConfiguration path, IWordsDictionary dictionary, IMessageCleanup cleanup)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            resourcesPath = Path.Combine(path.LexiconPath, "Repair");
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.cleanup = cleanup ?? throw new ArgumentNullException(nameof(cleanup));
            Load();
        }

        public string Repair(string sentence)
        {
            // first do not remove sentiment words
            // second it clashes with anther emoticon imlementations
            // third maybe we should replace with masks, but not so generic.
            if (string.IsNullOrEmpty(sentence))
            {
                return string.Empty;
            }

            foreach (var sentenceRepair in repairs)
            {
                sentence = sentenceRepair.Repair(sentence);
            }

            return cleanup.Cleanup(sentence); 
        }

        private void Load()
        {
            var document = XDocument.Load(Path.Combine(resourcesPath, "Repair.xml"));
            var items = from item in document.XPathSelectElements("//Rules/Rule")
                        select item;
            foreach (var item in items)
            {
                var test = item.Element("Test");
                if (test == null)
                {
                    throw new NullReferenceException("test");
                }

                var match = item.Element("Match");
                if (match == null)
                {
                    throw new NullReferenceException("match");
                }

                var replace = item.Element("Replace");
                if (replace == null)
                {
                    throw new NullReferenceException("Replace");
                }

                var repair = new SentenceRepair(dictionary, test.Value, match.Value, replace.Value);
                var verify = item.Element("Verify");
                if (verify != null)
                {
                    repair.VerifyDictionary = verify.Value.Split(';')
                        .Select(int.Parse)
                        .ToArray();
                }

                repairs.Add(repair);
            }
        }
    }
}
