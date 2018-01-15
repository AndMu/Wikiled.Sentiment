namespace Wikiled.Sentiment.Text.Configuration
{
    public interface IExtendedLexiconFactory : ILexiconFactory
    {
        string ResourcesPath { get; }
    }
}
