using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class AspectDectector : IAspectDectector
    {
        private readonly Dictionary<string, IWordItem> attributesTable = new Dictionary<string, IWordItem>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, IWordItem> aspectsTable = new Dictionary<string, IWordItem>(StringComparer.OrdinalIgnoreCase);

        public AspectDectector(IWordItem[] aspects, IWordItem[] attributes)
        {
            if (aspects is null)
            {
                throw new ArgumentNullException(nameof(aspects));
            }

            if (attributes is null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

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
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            aspectsTable.Remove(feature.Text);
        }

        public void AddFeature(IWordItem feature)
        {
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            aspectsTable[feature.Text] = feature;
        }

        public bool IsAspect(IWordItem word)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            return aspectsTable.TryGetWordValue(word, out _);
        }

        public bool IsAttribute(IWordItem word)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            return attributesTable.TryGetWordValue(word, out _);
        }

        public IEnumerable<IWordItem> AllFeatures => aspectsTable.Values;

        public IEnumerable<IWordItem> AllAttributes => attributesTable.Values;
    }
}
