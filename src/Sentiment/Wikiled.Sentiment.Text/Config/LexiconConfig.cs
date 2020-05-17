using System;
using System.IO;

namespace Wikiled.Sentiment.Text.Config
{
    public class LexiconConfig : ILexiconConfig
    {
        public string Resources { get; set; }

        public string NlpModels { get; set; }

        public LocationConfig Model { get; set; }

        public LocationConfig Lexicons { get; set; }

        public string GetFullPath(Func<ILexiconConfig, LocationConfig> config)
        {
            return Path.Combine(Resources ?? string.Empty, config(this).Local);
        }
    }
}
