using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Wikiled.Arff.Logic;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class XmlProcessingDataLoader
    {
        public IProcessingData LoadOldXml(string path)
        {
            var doc = XDocument.Load(path);
            var data = new ProcessingData();
            foreach (var item in GetRecords(doc.Descendants("Positive")))
            {
                data.Add(PositivityType.Positive, item);
            }

            foreach (var item in GetRecords(doc.Descendants("Negative")))
            {
                data.Add(PositivityType.Negative, item);
            }

            foreach (var item in GetRecords(doc.Descendants("Neutral")))
            {
                data.Add(PositivityType.Neutral, item);
            }

            return new StaticProcessingData(data);
        }

        private IEnumerable<SingleProcessingData> GetRecords(IEnumerable<XElement> elements)
        {
            foreach (var element in elements)
            {
                foreach (var document in element.Descendants("DataItem"))
                {
                    yield return GetRecord(document);
                }
            }
        }

        private SingleProcessingData GetRecord(XElement element)
        {
            var text = element.Descendants("Text").Select(x => x.Value).OrderByDescending(x => x.Length).FirstOrDefault();
            var item = new SingleProcessingData(text);
            var stars = element.Descendants("Stars").Select(x => double.Parse(x.Value)).OrderBy(x => x).FirstOrDefault();
            item.Stars = stars;
            var date = element.Descendants("Date").Select(x => DateTime.Parse(x.Value)).OrderBy(x => x).FirstOrDefault();
            item.Date = date;
            return item;
        }
    }
}
