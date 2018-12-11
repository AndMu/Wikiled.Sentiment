using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Resources;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class LexiconConfiguration : ILexiconConfiguration
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<LexiconConfiguration>();

        public LexiconConfiguration(IConfigurationHandler configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ResourcePath = configuration.ResolvePath("Resources");
            LexiconPath = Path.Combine(ResourcePath, configuration.SafeGetConfiguration("Lexicon", @"Library/Standard"));
            if (!Directory.Exists(LexiconPath))
            {
                log.LogError("Path doesn't exist: {0}", LexiconPath);
                throw new InvalidOperationException("Lexicon can't be constructed");
            }
        }

        public string LexiconPath { get; }

        public string ResourcePath { get; }
    }
}
