using System;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Text.NLP
{
    public class TextSplitterFactoryFinder
    {
        private readonly IStanfordFactory stanfordFactory;

        private readonly ICacheFactory cacheFactory;

        public TextSplitterFactoryFinder(IStanfordFactory stanfordFactory, ICacheFactory cacheFactory)
        {
            Guard.NotNull(() => stanfordFactory, stanfordFactory);
            Guard.NotNull(() => cacheFactory, cacheFactory);
            this.stanfordFactory = stanfordFactory;
            this.cacheFactory = cacheFactory;
        }

        public ISplitterFactory Load(
            POSTaggerType value,
            ILexiconFactory lexiconFactory,
            IConfigurationHandler configuration)
        {
            switch (value)
            {
                case POSTaggerType.Simple:
                    return new SimpleSplitterFactory(lexiconFactory);
                case POSTaggerType.Stanford:
                    return stanfordFactory.Create(lexiconFactory, configuration);
                case POSTaggerType.SharpNLP:
                    return new OpenNlpSplitterFactory(configuration.ResolvePath("Resources"), lexiconFactory, cacheFactory);
                default:
                    throw new NotSupportedException(value.ToString());

            }
        }
    }
}
