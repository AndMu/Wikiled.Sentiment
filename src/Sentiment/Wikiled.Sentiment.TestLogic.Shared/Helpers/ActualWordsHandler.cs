﻿using NUnit.Framework;
using System.IO;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Resources;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class ActualWordsHandler
    {
        private readonly IGlobalContainer container;

        public ActualWordsHandler(POSTaggerType type)
        {
            var factory = MainContainerFactory.Setup()
                .Config(configuration =>
                {
                    var resources = configuration.GetConfiguration("Resources");
                    var resourcesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, resources);
                    configuration.SetConfiguration("Resources", resourcesPath);
                })
                .Splitter(type)
                .SetupNullCache();

            container = factory.Create();
            Container = container.StartSession();
            Loader = new DocumentLoader(Container);
        }

        public void Reset()
        {
            Container = container.StartSession();
        }

        public static ActualWordsHandler InstanceSimple { get; } = new ActualWordsHandler(POSTaggerType.Simple);

        public static ActualWordsHandler InstanceOpen { get; } = new ActualWordsHandler(POSTaggerType.SharpNLP);

        public ISessionContainer Container { get; private set; }

        public IConfigurationHandler Configuration => Container.Resolve<IConfigurationHandler>();

        public IContextWordsHandler WordsHandler => Container.Resolve<IContextWordsHandler>();

        public DocumentLoader Loader { get; }

        public ITextSplitter TextSplitter => Container.Resolve<ITextSplitter>();

        public IWordFactory WordFactory => Container.Resolve<IWordFactory>();
    }
}
