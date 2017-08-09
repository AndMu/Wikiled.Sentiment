using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    public static class AmazonSentiment
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void ParseAndSave(this IEnumerable<Document> documents, string outFile, int skip = 0, int? total = null)
        {
            List<SingleProcessingData> positive = new List<SingleProcessingData>();
            List<SingleProcessingData> negative = new List<SingleProcessingData>();

            foreach (var currentDocument in documents)
            {
                var single = new SingleProcessingData(currentDocument);
                single.Date = currentDocument.DocumentTime.Value;
                single.Stars = currentDocument.Stars.Value;
                single.Document = currentDocument;
                if (currentDocument.Stars.Value > 3.5)
                {
                    positive.Add(single);
                }
                else
                {
                    negative.Add(single);
                }

                log.Info("Adding Review. Postive: {0} Negative: {1}", positive.Count, negative.Count);

                if (total != null ||
                    skip > 0)
                {
                    if (positive.Count >= skip + total &&
                        negative.Count >= (skip + total.Value))
                    {
                        break;
                    }
                }
            }

            log.Info("Saving Result: {0}", outFile);
            ProcessingData data = new ProcessingData();
            data.Positive = positive.Skip(skip).ToArray();
            data.Negative = negative.Skip(skip).ToArray();
            data.XmlSerialize().Save(outFile);
        }
    }
}
