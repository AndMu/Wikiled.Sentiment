using System.IO;
using System.Text.Json;

namespace Wikiled.Sentiment.Text.Config
{
    public static class LexiconConfigExtension
    {
        public static LexiconConfig Load(string location = null)
        {
            location ??= string.Empty;
            return JsonSerializer.Deserialize<LexiconConfig>(File.ReadAllBytes(Path.Combine(location, "lexicon.json")));
        }
    }
}
