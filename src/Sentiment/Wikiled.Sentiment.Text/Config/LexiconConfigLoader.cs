using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;

namespace Wikiled.Sentiment.Text.Config
{
    public class LexiconConfigLoader
    {
        private readonly ILogger<LexiconConfigLoader> log;

        public LexiconConfigLoader(ILogger<LexiconConfigLoader> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public ILexiconConfig Load(string root = null, string fileName = "lexicon.json")
        {
            root ??= string.Empty;
            var fileLocation = Path.Combine(root, fileName);
            log.LogInformation("Load configuration {0}...", fileLocation);
            var config = JsonSerializer.Deserialize<LexiconConfig>(File.ReadAllBytes(fileLocation));
            if (!string.IsNullOrEmpty(root))
            {
                config.Resources = Path.Combine(root, config.Resources);
            }

            return config;
        }
    }
}
