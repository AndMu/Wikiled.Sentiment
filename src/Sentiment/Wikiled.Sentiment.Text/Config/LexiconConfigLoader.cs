using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Common.Utilities.Resources.Config;

namespace Wikiled.Sentiment.Text.Config
{
    public class LexiconConfigLoader
    {
        private readonly ILogger<LexiconConfigLoader> log;

        private readonly IDataDownloader dataDownloader;

        public LexiconConfigLoader(ILogger<LexiconConfigLoader> log, IDataDownloader loader)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            dataDownloader = loader ?? throw new ArgumentNullException(nameof(loader));
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

        public async Task Download(ILexiconConfig config, string location = null)
        {
            if (Directory.Exists(config.GetFullPath(item => item.Model)))
            {
                log.LogInformation("Resources folder {0} found.", config.Resources);
            }
            else
            {
                if (config.Model == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(config.Model));
                }

                await dataDownloader.DownloadFile(new Uri(config.Model.Remote), config.Resources).ConfigureAwait(false);
            }

            if (config.Lexicons?.Remote != null)
            {
                await dataDownloader.DownloadFile(new Uri(config.Lexicons.Remote), config.Resources, true).ConfigureAwait(false);
            }
        }
    }
}
