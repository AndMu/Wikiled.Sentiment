using System;

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

            var data = new JsonProcessingDataLoader(path);
            data.Load();
            return data;
        }
    }
}
