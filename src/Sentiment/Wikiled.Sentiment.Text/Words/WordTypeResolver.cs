using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Text.Analysis.Words;

namespace Wikiled.Sentiment.Text.Words
{
    public class WordTypeResolver : IWordTypeResolver
    {
        private readonly WordsDictionary conjunctiveAdverbs;

        private readonly WordsDictionary coordinatingConjunctions;

        private readonly WordsDictionary subordinateConjunction;

        private readonly WordsDictionary invertingConjunction;

        private readonly WordsDictionary regularConjunction;

        private readonly WordsDictionary adverbs;

        private readonly WordsDictionary adjective;

        private readonly WordsDictionary article;

        private readonly WordsDictionary noun;

        private readonly WordsDictionary preposition;

        private readonly WordsDictionary pronoun;

        private readonly WordsDictionary verb;

        private WordTypeResolver()
        {
            conjunctiveAdverbs = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Conjunctions.ConjunctiveAdverb.txt");
            coordinatingConjunctions = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Conjunctions.CoordinatingConjunction.txt");
            subordinateConjunction = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Conjunctions.SubordinateConjunction.txt");
            invertingConjunction = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Conjunctions.InvertingConjunction.txt");
            regularConjunction = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Conjunctions.RegularConjunction.txt");
            adverbs = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Adverb.txt");
            adjective = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Adjective.txt");
            article = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Article.txt");
            noun = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Noun.txt");
            preposition = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Preposition.txt");
            pronoun = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Pronoun.txt");
            verb = WordsDictionary.ConstructFromInternalStream(@"Resources.POS.Verb.txt");
        }

        public static IWordTypeResolver Instance { get; } = new WordTypeResolver();

        public bool IsAdverb(string text)
        {
            return adverbs.Contains(text);
        }

        public bool IsVerb(string text)
        {
            return verb.Contains(text);
        }

        public bool IsNoun(string text)
        {
            return noun.Contains(text);
        }

        public bool IsAdjective(string text)
        {
            return adjective.Contains(text);
        }

        public bool IsArticle(string text)
        {
            return article.Contains(text);
        }

        public bool IsPreposition(string text)
        {
            return preposition.Contains(text);
        }

        public bool IsPronoun(string text)
        {
            return pronoun.Contains(text);
        }

        public bool IsCoordinatingConjunctions(string text)
        {
            return coordinatingConjunctions.Contains(text);
        }

        public bool IsConjunctiveAdverbs(string text)
        {
            return conjunctiveAdverbs.Contains(text);
        }

        public bool IsSubordinateConjunction(string text)
        {
            return subordinateConjunction.Contains(text);
        }

        public bool IsInvertingConjunction(string text)
        {
            return invertingConjunction.Contains(text);
        }

        public bool IsRegularConjunction(string text)
        {
            return regularConjunction.Contains(text);
        }

        public bool IsConjunction(string word)
        {
            return IsInvertingConjunction(word) ||
                   IsRegularConjunction(word) ||
                   IsSubordinateConjunction(word);
        }

        public bool IsSpecialEndSymbol(string word)
        {
            return word.IndexOf("??") >= 0 ||
                   word.IndexOf("!!") >= 0 ||
                   word.IndexOf("...") >= 0;
        }
    }
}
