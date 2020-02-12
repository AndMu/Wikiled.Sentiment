using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Common.Utilities.Resources;

namespace Wikiled.Sentiment.Text.Config
{
    public static class LexiconConfigExtension
    {
        private static ILogger log = ApplicationLogging.CreateLogger("LexiconConfigExtension");

        public static LexiconConfig Load(string location = null)
        {
            location ??= string.Empty;
            return JsonSerializer.Deserialize<LexiconConfig>(File.ReadAllBytes(Path.Combine(location, "lexicon.json")));
        }

        public static async Task<LexiconConfig> Download(string location = null)
        {
            var config = Load(location);
            if (Directory.Exists(config.Resources))
            {
                log.LogInformation("Resources folder {0} found.", config.Resources);
            }
            else
            {
                var dataDownloader = new DataDownloader(ApplicationLogging.LoggerFactory);
                await dataDownloader.DownloadFile(new Uri(config.Remote), config.Lexicon);
            }

            return config;
        }
    }
}
