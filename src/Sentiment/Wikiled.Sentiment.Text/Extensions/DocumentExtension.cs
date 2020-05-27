using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Arff.Logic;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Light;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class DocumentExtension
    {
        private static readonly Dictionary<string, NamedEntities> entityCache;

        static DocumentExtension()
        {
            entityCache = Enum.GetValues(typeof(NamedEntities)).Cast<NamedEntities>()
                .ToDictionary(item => item.ToString(), item => item, StringComparer.OrdinalIgnoreCase);
        }

        public static Document Construct(this LightDocument document, IWordFactory factory)
        {
            var result = new Document(document.Text);
            result.Author = document.Author;
            result.DocumentTime = document.DocumentTime;
            result.Id = document.Id;
            document.Title = document.Title;

            foreach (var sentence in document.Sentences)
            {
                var resultSentence = new SentenceItem(sentence.Text);
                result.Add(resultSentence, false);
                if (sentence.Words != null)
                {
                    for (var i = 0; i < sentence.Words.Length; i++)
                    {
                        var word = sentence.Words[i];
                        var wordItem = factory.CreateWord(word.Text, word.Tag);
                        wordItem.WordIndex = i;
                        WordEx wordData = WordExFactory.Construct(wordItem);
                        wordData.Phrase = word.Phrase;
                        if (!string.IsNullOrEmpty(word.Entity))
                        {
                            if (entityCache.TryGetValue(word.Entity, out var entity))
                            {
                                wordData.EntityType = entity;
                            }
                            else
                            {
                                wordData.CustomEntity = word.Entity;
                            }
                        }
                        else
                        {
                            wordData.EntityType = NamedEntities.None;
                        }

                        resultSentence.Add(wordData);
                    }
                }
            }

            return result;
        }

        public static PositivityType? GetPositivity(this Document document)
        {
            if (document?.Stars == null)
            {
                return null;
            }

            return document.Stars > 3 ? PositivityType.Positive : PositivityType.Negative;
        }
    }
}
