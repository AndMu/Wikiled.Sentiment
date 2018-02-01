using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NLog;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Resources;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Features.Results;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Features
{
    public class FeatureIndicator : IFeatureIndicator
    {
        private readonly Dictionary<string, List<DetectionItem>> detectionTable = new Dictionary<string, List<DetectionItem>>(StringComparer.OrdinalIgnoreCase);

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private DetectionData originalData;

        public FeatureIndicator()
        {
            Load();
        }

        public IIdentificationResult CheckIndication(IWordItem word)
        {
            List<DetectionItem> list;
            if (detectionTable.TryGetWordValue(word, out list))
            {
                log.Debug("Found <{0}> results: {1}", word.Text, list.Count);
                return new IdentificationResult(list);
            }

            return null;
        }

        private void Load()
        {
            XDocument document = GetType().LoadXmlData("Resources.Features.Features.xml");
            originalData = document.XmlDeserialize<DetectionData>();
            foreach (var adjectives in originalData.Adjectives)
            {
                foreach (var detectionItem in adjectives.Words)
                {
                    detectionItem.Block = adjectives;
                    detectionTable.GetItemCreate(detectionItem.Text).Add(detectionItem);
                }

                DetectionItem item = new DetectionItem
                                     {
                                         Block = adjectives,
                                         Probability = 1,
                                         Text = "Empty"
                                     };

                detectionTable.GetItemCreate(adjectives.Name).Add(item);
            }
        }
    }
}
