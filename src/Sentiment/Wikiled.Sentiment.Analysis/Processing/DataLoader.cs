using System;
using System.IO;
using Newtonsoft.Json;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class DataLoader
    {
        public IProcessingData Load(string path)
        {
            if (path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return new XmlProcessingDataLoader().LoadOldXml(path);
            }

            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ProcessingData>(data);
        }
    }
}
