using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.Dictionary.Streams;
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
            conjunctiveAdverbs = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Conjunctions.ConjunctiveAdverb.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            coordinatingConjunctions = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Conjunctions.CoordinatingConjunction.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            subordinateConjunction = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Conjunctions.SubordinateConjunction.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            invertingConjunction = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Conjunctions.InvertingConjunction.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            regularConjunction = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Conjunctions.RegularConjunction.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            adverbs = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Adverb.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            adjective = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Adjective.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            article = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Article.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            noun = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Noun.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            preposition = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Preposition.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            pronoun = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Pronoun.txt", new EmbeddedStreamSource<WordTypeResolver>()));
            verb = WordsDictionary.Construct(new DictionaryStream(@"Resources.POS.Verb.txt", new EmbeddedStreamSource<WordTypeResolver>()));
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
