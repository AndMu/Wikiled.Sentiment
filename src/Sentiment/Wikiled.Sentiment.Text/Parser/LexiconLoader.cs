﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Common.Utilities.Resources.Config;
using Wikiled.Sentiment.Text.Config;

namespace Wikiled.Sentiment.Text.Parser
{
    public class LexiconLoader : ILexiconLoader
    {
        private readonly ILogger<LexiconLoader> logger;

        private Dictionary<string, ISentimentDataHolder> table;

        private readonly ILexiconConfig config;

        public LexiconLoader(ILogger<LexiconLoader> logger, ILexiconConfig config)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IEnumerable<string> Supported => table.Select(item => item.Key);

        public void Load()
        {
            if (string.IsNullOrEmpty(config?.Lexicons?.Local))
            {
                logger.LogWarning("Lexicons not found");
                return;
            }

            var path = config.GetFullPath(item => item.Lexicons);
            logger.LogInformation("Loading lexicons: {0}", path);
            table = new Dictionary<string, ISentimentDataHolder>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in Directory.GetFiles(path))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var holder = SentimentDataHolder.Load(file);
                table[name] = holder;
            }

            logger.LogInformation("Loaded {0} lexicons", table.Count);
        }

        public ISentimentDataHolder GetLexicon(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            logger.LogDebug("Get lexicon: {0}", name);
            if (!table.TryGetValue(name, out var value))
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Lexicon not found: " + name);
            }

            return value;
        }
    }
}
