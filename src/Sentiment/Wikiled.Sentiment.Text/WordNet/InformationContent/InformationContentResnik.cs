using System;
using System.Collections.Generic;
using System.IO;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Text.WordNet.InformationContent
{
    public class InformationContentResnik : IInformationContentResnik
    {
        readonly Dictionary<string, double> frequencies = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        private InformationContentResnik()
        {
        }

        public double GetIC(SynSet synSet)
        {
            var value = GetFrequency(synSet);
            double total = synSet.POS == WordType.Noun ? TotalNouns : TotalVerbs;
            return -Math.Log(value / total, 10);
        }

        public double GetFrequency(SynSet synSet)
        {
            if (synSet == null)
            {
                throw new ArgumentNullException("synSet");
            }

            return frequencies[synSet.Offset + MapPos(synSet.POS)];
        }

        private static string MapPos(WordType pos)
        {
            switch (pos)
            {
                case WordType.Verb:
                    return "v";
                case WordType.Noun:
                    return "n";
                default:
                    throw new ArgumentOutOfRangeException("pos", string.Format("Synset for POS {0} not supported", pos));
            }
        }

        public static InformationContentResnik Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            InformationContentResnik contentResnik = new InformationContentResnik();
            using (StreamReader streamReader = new StreamReader(path))
            {
                string line;
                bool contentReached = false;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!contentReached &&
                        line.IndexOf("wnver", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        contentReached = true;
                        continue;
                    }

                    var blocks = line.Split(' ');
                    if (blocks.Length < 2)
                    {
                        throw new ArgumentOutOfRangeException("path", "Can't parse line: " + line);
                    }

                    var id = blocks[0];
                    var total = double.Parse(blocks[1]);
                    contentResnik.frequencies[id] = total;
                    if (blocks.Length == 3)
                    {
                        var pos = id.Substring(id.Length - 1);
                        double totalOut;
                        contentResnik.frequencies.TryGetValue(pos, out totalOut);
                        contentResnik.frequencies[pos] = totalOut + total;
                    }
                }
            }

            return contentResnik;
        }

        public double TotalNouns => frequencies[MapPos(WordType.Noun)];

        public double TotalVerbs => frequencies[MapPos(WordType.Verb)];
    }
}
