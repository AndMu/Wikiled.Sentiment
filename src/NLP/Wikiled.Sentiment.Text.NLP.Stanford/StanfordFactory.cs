using System;
using System.IO;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Text.Analysis.Cache;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    public class StanfordFactory : IStanfordFactory
    {
        private readonly ICacheFactory cache;

        public StanfordFactory(ICacheFactory cache)
        {
            Guard.NotNull(() => cache, cache);
            this.cache = cache;
        }

        public ISplitterFactory Create(ILexiconFactory lexiconFactory, IConfigurationHandler configuration)
        {
            DirectoryInfo info = new DirectoryInfo(configuration.GetConfiguration("Stanford"));
            if (!info.Exists)
            {
                throw new ArgumentOutOfRangeException("Directory does not exist " + info.FullName);
            }

            return new StanfordSplitterFactory(
                info.FullName,
                lexiconFactory,
                cache);
        }
    }
}
