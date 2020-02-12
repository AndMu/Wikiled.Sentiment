namespace Wikiled.Sentiment.Text.Config
{
    public interface ILexiconConfig
    {
        string Lexicon { get; }

        string Resources { get; }

        string Remote { get; }

        string FullLexiconPath { get; }
    }
}