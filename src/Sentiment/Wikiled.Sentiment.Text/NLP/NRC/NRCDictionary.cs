using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Persitency;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Resources;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.NRC
{
    public class NRCDictionary
    {
        public static NRCDictionary Instance { get; } = new NRCDictionary();

        private readonly Dictionary<string, NRCRecord> table = new Dictionary<string, NRCRecord>(StringComparer.OrdinalIgnoreCase);

        private NRCDictionary()
        {
            ReadDataFromInternalStream("Resources.Dictionary.NRC.txt");
        }

        public IEnumerable<WordNRCRecord> AllRecords => table.Select(item => new WordNRCRecord(item.Key, item.Value));

        public NRCRecord FindRecord(WordEx word)
        {
            IWordItem item = word.UnderlyingWord as IWordItem;
            return item != null ? FindRecord(item) : FindRecord(word.Text);
        }

        public NRCRecord FindRecord(IWordItem word)
        {
            NRCRecord nrcRecord;
            if (!table.TryGetWordValue(word, out nrcRecord))
            {
                return null;
            }

            if (nrcRecord == null)
            {
                return null;
            }

            nrcRecord = (NRCRecord)nrcRecord.Clone();
            if (word.Relationship?.Inverted != null)
            {
                nrcRecord.Invert();
            }

            return nrcRecord;
        }

        public NRCRecord FindRecord(string word)
        {
            NRCRecord nrcRecord;
            table.TryGetValue(word, out nrcRecord);
            return (NRCRecord)nrcRecord?.Clone();
        }

        private void ReadDataFromInternalStream(string name)
        {
            using (StreamReader reader = new StreamReader(typeof(NRCDictionary).GetEmbeddedFile(name)))
            {
                Func<string, string> converterText = data => data;
                Func<string, int> converterDouble = int.Parse;
                ReadTabResourceDataFile boosterData = new ReadTabResourceDataFile(reader);
                boosterData.UseDefaultIfNotFound = false;
                int index = 0;
                foreach (var record in boosterData.ReadData(converterText, converterDouble))
                {
                    NRCRecord nrcRecord;
                    if (!table.TryGetValue(record.Item1, out nrcRecord))
                    {
                        nrcRecord = new NRCRecord();
                        table[record.Item1] = nrcRecord;
                        index = 0;
                    }

                    index++;
                    if (record.Item2 == 0)
                    {
                        continue;
                    }

                    switch (index)
                    {
                        case 1:
                            nrcRecord.IsAnger = true;
                            break;
                        case 2:
                            nrcRecord.IsAnticipation = true;
                            break;
                        case 3:
                            nrcRecord.IsDisgust = true;
                            break;
                        case 4:
                            nrcRecord.IsFear = true;
                            break;
                        case 5:
                            nrcRecord.IsJoy = true;
                            break;
                        case 6:
                            nrcRecord.IsNegative = true;
                            break;
                        case 7:
                            nrcRecord.IsPositive = true;
                            break;
                        case 8:
                            nrcRecord.IsSadness = true;
                            break;
                        case 9:
                            nrcRecord.IsSurprise = true;
                            break;
                        case 10:
                            nrcRecord.IsTrust = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("index", index.ToString());
                    }
                }
            }
        }
    }
}
