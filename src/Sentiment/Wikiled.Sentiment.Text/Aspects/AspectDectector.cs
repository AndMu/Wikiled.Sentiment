using System;
using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectDectector : IAspectDectector
    {
        private readonly Dictionary<string, IWordItem> attributesTable = new Dictionary<string, IWordItem>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, IWordItem> aspectsTable = new Dictionary<string, IWordItem>(StringComparer.OrdinalIgnoreCase);

        public AspectDectector(IWordItem[] aspects, IWordItem[] attributes)
        {
            Guard.NotNull(() => attributes, attributes);
            Guard.NotNull(() => aspects, aspects);
            foreach (var attribute in attributes)
            {
                attributesTable[attribute.Text] = attribute;
            }

            foreach (var aspect in aspects)
            {
                aspectsTable[aspect.Text] = aspect;
            }
        }

        public void Remove(IWordItem feature)
        {
            Guard.NotNull(() => feature, feature);
            aspectsTable.Remove(feature.Text);
        }

        public void AddFeature(IWordItem feature)
        {
            Guard.NotNull(() => feature, feature);
            aspectsTable[feature.Text] = feature;
        }

        public bool IsAspect(IWordItem word)
        {
            Guard.NotNull(() => word, word);
            IWordItem result;
            return aspectsTable.TryGetWordValue(word, out result);
        }

        public bool IsAttribute(IWordItem word)
        {
            Guard.NotNull(() => word, word);
            IWordItem result;
            return attributesTable.TryGetWordValue(word, out result);
        }

        public IEnumerable<IWordItem> AllFeatures => aspectsTable.Values;

        public IEnumerable<IWordItem> AllAttributes => attributesTable.Values;
    }
}
