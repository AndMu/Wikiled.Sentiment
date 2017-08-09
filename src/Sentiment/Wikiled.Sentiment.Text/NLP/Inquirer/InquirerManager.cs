using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    public class InquirerManager
    {
        private readonly Dictionary<string, List<InquirerRecord>> items = new Dictionary<string, List<InquirerRecord>>(StringComparer.OrdinalIgnoreCase);

        private static InquirerManager loaded;

        private static readonly object syncRoot = new object();

        public int Total => items.Count;

        public static InquirerManager GetLoaded()
        {
            lock(syncRoot)
            {
                if (loaded != null)
                {
                    return loaded;
                }

                loaded = new InquirerManager();
                loaded.Load();
                return loaded;
            }
        }

        public InquirerDefinition GetDefinitions(WordEx word)
        {
            IWordItem item = word.UnderlyingWord as IWordItem;
            return item != null ? GetDefinitions(item) : GetDefinitions(word.Text);
        }

        public InquirerDefinition GetDefinitions(IWordItem word)
        {
            Guard.NotNull(() => word, word);
            List<InquirerRecord> definitions;
            if (items.TryGetWordValue(word, out definitions))
            {
                return new InquirerDefinition(word.Text, definitions.ToArray());
            }

            return new InquirerDefinition(word.Text, new InquirerRecord[] {});
        }

        public InquirerDefinition GetDefinitions(string word)
        {
            Guard.NotNullOrEmpty(() => word, word);
            List<InquirerRecord> definitions;
            if (items.TryGetValue(word, out definitions))
            {
                return new InquirerDefinition(word, definitions.ToArray());
            }

            return new InquirerDefinition(word, new InquirerRecord[] {});
        }

        public void Load()
        {
            ReadDataFromInternalStream();
        }

        private void ReadDataFromInternalStream()
        {
            using (BinaryReader reader = new BinaryReader(typeof(InquirerManager).GetEmbeddedFile(@"Resources.Dictionary.inquirer.dat")))
            {
                byte[] data = new byte[reader.BaseStream.Length];
                reader.Read(data, 0, data.Length);
                var unzipedText = data.UnZipString();
                int i = 0;
                foreach (var line in unzipedText.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries))
                {
                    i++;
                    if (i == 1)
                    {
                        continue;
                    }

                    var entries = line.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                    string word = string.Intern(entries[0]);
                    int idIndex = word.IndexOf("#");
                    if (idIndex > 0)
                    {
                        word = word.Substring(0, idIndex);
                    }

                    items.GetSafeCreate(word).Add(new InquirerRecord(word, entries.Skip(1).ToArray()));
                }
            }
        }
    }
}
