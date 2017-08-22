using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Core.Utility.Resources;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    public class InquirerManager
    {
        private static readonly Lazy<InquirerManager> loaded;

        private readonly Dictionary<string, List<InquirerRecord>> items = new Dictionary<string, List<InquirerRecord>>(StringComparer.OrdinalIgnoreCase);

        static InquirerManager()
        {
            loaded = new Lazy<InquirerManager>(
                () =>
                    {
                        var instance = new InquirerManager();
                        instance.Load();
                        return instance;
                    });
        }

        public int Total => items.Count;

        public static InquirerManager GetLoaded()
        {
            return loaded.Value;
        }

        public InquirerDefinition GetDefinitions(string word)
        {
            Guard.NotNullOrEmpty(() => word, word);
            List<InquirerRecord> definitions;
            if (items.TryGetValue(word, out definitions))
            {
                return new InquirerDefinition(word, definitions.ToArray());
            }

            return new InquirerDefinition(word, new InquirerRecord[] { });
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
                foreach (var line in unzipedText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    i++;
                    if (i == 1)
                    {
                        continue;
                    }

                    var entries = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
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
