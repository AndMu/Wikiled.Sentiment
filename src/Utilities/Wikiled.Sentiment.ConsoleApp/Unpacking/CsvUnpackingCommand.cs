using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Logging;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    /// <summary>
    /// unpackcsv -Source=c:\Source\Data\DataSets\Market\market_k8.txt -Out=\Source\Data
    /// </summary>
    public class CsvUnpackingCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public string Out { get; set; }

        [Required]
        public string Source { get; set; }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(Out))
            {
                Out = Source;
            }

            log.Info("Starting unpacking from {0} to {1}", Source, Out);
            if (!File.Exists(Source))
            {
                throw new ApplicationException("Can't find " + Source);
            }

            Out.EnsureDirectoryExistence();
            ProcessingData simple = new ProcessingData();
            List<SingleProcessingData> datas = new List<SingleProcessingData>();

            using (var file = new StreamReader(Source))
            {
                string line;
                StringBuilder builder = new StringBuilder();
                while ((line = file.ReadLine()) != null)
                {
                    if (line.IndexOf("EVENTS:") >= 0)
                    {
                        AddItem(builder, datas);
                    }

                    builder.Append(line);
                    if (datas.Count > 5000)
                    {
                        break;
                    }
                }

                AddItem(builder, datas);
            }

            simple.Positive = datas.ToArray();
            simple.XmlSerialize().Save(Path.Combine(Out, "articles.xml"));
        }

        private static void AddItem(StringBuilder builder, List<SingleProcessingData> datas)
        {
            if (builder.Length == 0)
            {
                return;
            }

            SingleProcessingData data = new SingleProcessingData(new Document(builder.ToString()));
            data.Date = DateTime.Now;
            data.Stars = 3;
            datas.Add(data);
            builder.Clear();
        }
    }
}
