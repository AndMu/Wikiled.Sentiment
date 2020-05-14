using System;

namespace Wikiled.Sentiment.Text.Config
{
    public interface ILexiconConfig
    {
        string Resources { get; }

        string NlpModels { get; }

        LocationConfig Model { get; }

        LocationConfig Lexicons { get; }

        string GetFullPath(Func<ILexiconConfig, LocationConfig> config);
    }
}