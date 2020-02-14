using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Resources;

namespace Wikiled.Sentiment.Text.Config
{
    public class LexiconConfigLoader
    {
        private ILogger<LexiconConfigLoader> log;

        public LexiconConfigLoader(ILogger<LexiconConfigLoader> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public LexiconConfig Load(string location = null)
        {
            location ??= string.Empty;
            location = Path.Combine(location, "lexicon.json");
            log.LogInformation("Load configuration {0}...", location);
            return JsonSerializer.Deserialize<LexiconConfig>(File.ReadAllBytes(location));
        }

        public async Task<LexiconConfig> Download(string location = null)
        {
            var config = Load(location);
            if (Directory.Exists(config.Resources))
            {
                log.LogInformation("Resources folder {0} found.", config.Resources);
            }
            else
            {
                var dataDownloader = new DataDownloader(ApplicationLogging.LoggerFactory);
                await dataDownloader.DownloadFile(new Uri(config.Remote), config.FullLexiconPath);
            }

            return config;
        }
    }
}
