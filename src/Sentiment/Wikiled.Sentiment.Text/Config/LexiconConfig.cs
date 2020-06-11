using System;
using System.IO;
using Wikiled.Common.Utilities.Resources.Config;

namespace Wikiled.Sentiment.Text.Config
{
    public class LexiconConfig : ILexiconConfig
    {
        public string Resources { get; set; }

        public string NlpModels { get; set; }

        public LocationConfig Model { get; set; }

        public LocationConfig Lexicons { get; set; }
    }
}
