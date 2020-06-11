using Wikiled.Common.Utilities.Resources.Config;

namespace Wikiled.Sentiment.Text.Config
{
    public interface ILexiconConfig : ILocalDownload
    {
        string NlpModels { get; }

        LocationConfig Model { get; }

        LocationConfig Lexicons { get; }
    }
}