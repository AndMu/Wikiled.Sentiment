using Autofac;
using Wikiled.Console.Arguments;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Processing.Persistency;

namespace Wikiled.Sentiment.ConsoleApp.Analysis.Config
{
    public abstract class BaseRawConfig : ICommandConfig, IDataSourceConfig
    {
        public string Weights { get; set; }

        public string All { get; set; }

        public string Positive { get; set; }

        public string Negative { get; set; }

        public bool UseBagOfWords { get; set; }

        public bool InvertOff { get; set; }

        public virtual void Build(ContainerBuilder builder)
        { 
            MainContainerFactory.Setup(builder)
                .Config()
                .SetupLocalCache()
                .Splitter()
                .Validate();
        }
    }
}
