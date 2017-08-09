using System;
using System.Collections.Generic;
using System.IO;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Persitency;
using Wikiled.Text.Analysis.Resources;

namespace Wikiled.Sentiment.Text.Helpers
{
    public class WordsDictionary
    {
        public static WordsDictionary Empty { get; } = new WordsDictionary(new Dictionary<string, double>());

        private WordsDictionary(Dictionary<string, double> table)
        {
            Guard.NotNull(() => table, table);
            RawData = table;
        }

        public Dictionary<string, double> RawData { get; }

        public static WordsDictionary ConstructFromInternalStream(string name)
        {
            return new WordsDictionary(ReadDataFromInternalStream(name));
        }

        public static WordsDictionary ConstructFromInternalZippedStream(string name)
        {
            return new WordsDictionary(ReadDataFromInternalZippedStream(name));
        }

        public bool Contains(string word)
        {
            Guard.NotNullOrEmpty(() => word, word);
            return RawData.ContainsKey(word);
        }

        private static Dictionary<string, double> ReadDataFromInternalStream(string name)
        {
            using(StreamReader reader = new StreamReader(typeof(WordsDictionary).GetEmbeddedFile(name)))
            {
                return ReadDataFromStream(reader);
            }
        }

        private static Dictionary<string, double> ReadDataFromInternalZippedStream(string name)
        {
            using(BinaryReader reader = new BinaryReader(typeof(WordsDictionary).GetEmbeddedFile(name)))
            {
                byte[] data = new byte[reader.BaseStream.Length];
                reader.Read(data, 0, data.Length);
                var unzipedText = data.UnZipString();
                using(TextReader textReader = new StringReader(unzipedText))
                {
                    return ReadDataFromStream(textReader);
                }
            }
        }

        private static Dictionary<string, double> ReadDataFromStream(TextReader reader)
        {
            Func<string, string> coverterText = data => data;
            Func<string, double> coverterDouble = double.Parse;
            ReadTabResourceDataFile boosterData = new ReadTabResourceDataFile(reader);
            boosterData.UseDefaultIfNotFound = true;
            return boosterData.ReadDataSafeDictionary(coverterText, coverterDouble, StringComparer.OrdinalIgnoreCase);
        }
    }
}
