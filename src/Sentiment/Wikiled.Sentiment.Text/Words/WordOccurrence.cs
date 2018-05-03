﻿using System;
using System.Collections.Generic;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.POS.Tags;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Data;

namespace Wikiled.Sentiment.Text.Words
{
    public class WordOccurrence : IWordItem
    {
        private NamedEntities entity;

        private WordOccurrence(string text, string raw, BasePOSType pos)
        {
            Guard.NotNullOrEmpty(() => text, text);
            Guard.NotNull(() => pos, pos);
            Guard.IsValid(() => pos, pos, type => !type.IsGroup, "Check group");
            Text = text;
            POS = pos;
            Stemmed = raw;
        }

        public IEnumerable<IWordItem> AllWords
        {
            get
            {
                yield return this;
            }
        }

        public NamedEntities Entity
        {
            get
            {
                if (!string.IsNullOrEmpty(Text) &&
                    Text[0] == '#')
                {
                    return NamedEntities.Hashtag;
                }

                return entity;
            }

            set => entity = value;
        }

        public bool IsFeature { get; private set; }

        public bool IsFixed => !string.IsNullOrEmpty(Text) &&
                               Text.IndexOf("xxxbad", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                               Text.IndexOf("xxxgood", StringComparison.CurrentCultureIgnoreCase) == 0;

        public bool IsInvertor { get; private set; }

        public bool IsQuestion { get; private set; }

        public bool IsSentiment { get; private set; }

        public bool IsSimple => true;

        public bool IsStopWord { get; private set; }

        public bool IsTopAttribute { get; private set; }

        public string NormalizedEntity { get; set; }

        public IWordItem Parent { get; set; }

        public BasePOSType POS { get; }

        public double? QuantValue { get; private set; }

        public IWordItemRelationships Relationship { get; private set; }

        public string Stemmed { get; }

        public string Text { get; }

        public int WordIndex { get; set; }

        public InquirerDefinition Inquirer { get; private set; }

        public static WordOccurrence Create(IWordsHandler wordsHandlers, string text, string raw, BasePOSType pos)
        {
            Guard.NotNull(() => wordsHandlers, wordsHandlers);
            text = text?.ToLower();
            string rawWord = string.IsNullOrEmpty(raw) ? wordsHandlers.Extractor.GetWord(text) : raw;
            rawWord = rawWord?.ToLower();
            var item = new WordOccurrence(text, rawWord, pos);
            item.Relationship = new WordItemRelationships(wordsHandlers, item);
            item.IsSentiment = wordsHandlers.IsSentiment(item);
            item.IsFeature = wordsHandlers.IsFeature(item);
            item.IsTopAttribute = wordsHandlers.AspectDectector.IsAttribute(item);
            item.QuantValue = wordsHandlers.MeasureQuantifier(item);
            item.IsInvertor = wordsHandlers.IsInvertAdverb(item);
            item.IsQuestion = wordsHandlers.IsQuestion(item);
            item.IsStopWord = wordsHandlers.IsStop(item);
            item.Inquirer = wordsHandlers.InquirerManager.GetDefinitions(text);
            return item;
        }

        public static WordOccurrence CreateBasic(string text, BasePOSType pos)
        {
            var item = new WordOccurrence(text, text, pos);
            item.Relationship = new WordItemRelationships(null, item);
            return item;
        }

        public override string ToString()
        {
            if (Relationship?.Sentiment != null)
            {
                return $"Word: [{Text}] [{POS.Tag}] [Sentiment:{Relationship.Sentiment.DataValue.Value}]";
            }

            return $"Word: [{Text}] [{POS.Tag}]";
        }

        public void Reset()
        {
            Parent?.Reset();
            Relationship?.Reset();
        }
    }
}
