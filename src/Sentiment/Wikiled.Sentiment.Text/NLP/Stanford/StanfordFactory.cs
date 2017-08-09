using Wikiled.Core.Utility.Resources;
using Wikiled.Sentiment.Text.Configuration;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    public class StanfordFactory : IStanfordFactory
    {
        public ISimpleTextSplitterFactory Create(ILexiconFactory lexiconFactory, IConfigurationHandler configuration)
        {
            return AssemblyLoader.CreateInstanceSafe<ISimpleTextSplitterFactory>(
                "Wikiled.Sentiment.Text.NLP.Stanford.StanfordTextSplitterFactory, Wikiled.Sentiment.Text.NLP.Stanford",
                new[]
                {
                    typeof (string),
                    typeof (ILexiconFactory)
                },
                new object[]
                {
                    GetStanfordPath(configuration),
                    lexiconFactory
                });
        }

        private static string GetStanfordPath(IConfigurationHandler configuration)
        {
            string path;
            if (!configuration.TryGetConfiguration("Stanford", out path))
            {
                path = ".";
            }
            else
            {
                AssembliesResolver.Instance.AddSearchPath(path);
            }

            return path;
        }
    }
}