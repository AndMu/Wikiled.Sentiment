using System;
using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Cache;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.WordNet.Engine;

namespace Wikiled.Sentiment.Text.WordNet.InformationContent
{
    public class JcnMeasure : IRelatednessMesaure
    {
        private readonly ICacheHandler cache;

        private readonly IWordNetEngine engine;

        private readonly IInformationContentResnik icResnik;

        public JcnMeasure(ICacheHandler cache, IInformationContentResnik icResnik, IWordNetEngine engine)
        {
            Guard.NotNull(() => cache, cache);
            Guard.NotNull(() => icResnik, icResnik);
            Guard.NotNull(() => engine, engine);
            this.icResnik = icResnik;
            this.engine = engine;
            this.cache = cache;
        }

        public static double MaxSimilarity => 1 / MinDistance;

        public static double MinDistance => 1;

        public double Measure(string word1, string word2, WordType type = WordType.Noun)
        {
            if(type != WordType.Noun &&
               type != WordType.Verb)
            {
                throw new ArgumentOutOfRangeException("type");
            }

            if(string.IsNullOrWhiteSpace(word1))
            {
                throw new ArgumentNullException("word1");
            }

            if(string.IsNullOrWhiteSpace(word2))
            {
                throw new ArgumentNullException("word2");
            }

            if(string.Compare(word1, word2, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return MaxSimilarity;
            }

            var synsets1 = engine.GetSynSets(word1, type);
            var synsets2 = engine.GetSynSets(word2, type);

            double relatedness = 0;
            foreach(var synSet1 in synsets1)
            {
                if(synsets2.Contains(synSet1))
                {
                    return MaxSimilarity;
                }
            }

            foreach(var synSet1 in synsets1)
            {
                foreach(var synSet2 in synsets2)
                {
                    var current = Measure(synSet1, synSet2);
                    if(current > relatedness)
                    {
                        relatedness = current;
                    }
                }
            }

            return relatedness;
        }

        public double Measure(SynSet synSet1, SynSet synSet2)
        {
            Guard.NotNull(() => synSet1, synSet1);
            Guard.NotNull(() => synSet2, synSet2);
            var tag = GenerateTag(synSet1, synSet2);
            double value;
            if (!cache.TryGetItem(tag, out value))
            {
                SynSetRelation[] vlist = new SynSetRelation[1];
                vlist[0] = SynSetRelation.Hypernym;
                var common = synSet1.GetClosestMutuallyReachableSynset(synSet2, vlist);
                double distance = 0;
                if (common != null)
                {
                    distance = icResnik.GetIC(synSet1) + icResnik.GetIC(synSet2) - 2*icResnik.GetIC(common);
                }

                if (distance == 0)
                {
                    return 0;
                }

                if (distance < MinDistance)
                {
                    distance = MinDistance;
                }

                value = 1/distance;
            }

            cache.SetItem(tag, value);
            return value;
        }

        public double Measure(IWordItem first, IWordItem second, WordType enforceType = WordType.Unknown)
        {
            Guard.NotNull(() => first, first);
            Guard.NotNull(() => second, second);

            double maxSim = 0;
            if(first == second ||
               first.Text == second.Text)
            {
                throw new ArgumentOutOfRangeException("first");
            }

            foreach(var wordOccurrence1 in GetWords(first))
            {
                if(enforceType == WordType.Unknown)
                {
                    if(wordOccurrence1.POS.WordType != WordType.Noun &&
                       wordOccurrence1.POS.WordType != WordType.Verb)
                    {
                        continue;
                    }
                }

                foreach(var wordOccurrence2 in GetWords(second))
                {
                    if(enforceType == WordType.Unknown)
                    {
                        if(wordOccurrence2.POS.WordType != WordType.Noun &&
                           wordOccurrence2.POS.WordType != WordType.Verb)
                        {
                            continue;
                        }
                        if(wordOccurrence1.POS.WordType != wordOccurrence2.POS.WordType)
                        {
                            continue;
                        }
                    }

                    var type = enforceType == WordType.Unknown ? wordOccurrence1.POS.WordType : enforceType;
                    var value = Measure(wordOccurrence1.Stemmed, wordOccurrence2.Stemmed, type);
                    if(value > maxSim)
                    {
                        maxSim = value;
                    }
                }
            }

            return maxSim;
        }

        private IEnumerable<IWordItem> GetWords(IWordItem word)
        {
            var phrase = word as IPhrase;
            if(phrase != null)
            {
                foreach(var child in phrase.AllWords)
                {
                    yield return child;
                }
            }
            else
            {
                yield return word;
            }
        }

        private string GenerateTag(SynSet synSet1, SynSet synSet2)
        {
            if(synSet1.Offset > synSet2.Offset)
            {
                return synSet1.Offset + ":" + synSet2.Offset;
            }

            return synSet2.Offset + ":" + synSet1.Offset;
        }
    }
}
