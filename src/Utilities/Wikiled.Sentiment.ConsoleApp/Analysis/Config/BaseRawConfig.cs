using Autofac;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.ConsoleApp.Analysis.Config
{
    public abstract class BaseRawConfig : ICommandConfig, IDataSourceConfig
    {
        public string Weights { get; set; }

        public bool Redis { get; set; }

        public int? Port { get; set; }

        public string All { get; set; }

        public string Positive { get; set; }

        public string Negative { get; set; }

        public string Input { get; set; }

        public bool UseBagOfWords { get; set; }

        public bool InvertOff { get; set; }

        public POSTaggerType Tagger { get; set; } = POSTaggerType.SharpNLP;

        public void Build(ContainerBuilder builder)
        { 
            MainContainerFactory.Setup(builder)
                .Config()
                .SetupLocalCache()
                .Splitter()
                .Validate();
        }
    }
}
