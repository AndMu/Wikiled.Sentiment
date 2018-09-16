using System;
using System.IO;
using NLog;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class LexiconConfiguration : ILexiconConfiguration
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public LexiconConfiguration(IConfigurationHandler configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var path = configuration.ResolvePath("Resources");
            LexiconPath = Path.Combine(path, configuration.SafeGetConfiguration("Lexicon", @"Library/Standard"));
            if (!Directory.Exists(LexiconPath))
            {
                log.Error("Path doesn't exist: {0}", LexiconPath);
                throw new InvalidOperationException("Lexicon can't be constructed");
            }
        }
       
        public string LexiconPath { get; }
    }
}
