using System;
using System.Reactive.Concurrency;
using Autofac;
using Autofac.Extras.AggregateService;
using Microsoft.Extensions.Caching.Memory;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Containers;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SentimentMainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<DefaultNlpModule>();
            
            builder.RegisterType<DataLoader>().As<IDataLoader>();
            builder.RegisterType<SessionContainer>().As<ISessionContainer>();
            builder.RegisterType<LexiconConfiguration>().As<ILexiconConfiguration>().SingleInstance();
            builder.RegisterType<InquirerManager>().As<IInquirerManager>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<ParsedReviewManager>().As<IParsedReviewManager>();

            builder.RegisterType<SentenceRepairHandler>().As<ISentenceRepairHandler>().SingleInstance();
            builder.RegisterType<ExtendedWords>().As<IExtendedWords>().SingleInstance();

            builder.Register(c => new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>().SingleInstance();

            builder.RegisterType<WordOccurenceFactory>().As<IWordFactory>();

            var parallel = Environment.ProcessorCount;
            if (parallel > 30)
            {
                parallel = 30;
            }

            builder.RegisterType<WordsHandler>().As<IWordsHandler>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<AspectSerializer>().As<IAspectSerializer>();
            builder.Register(item => new QueueTextSplitter(parallel, item.ResolveNamed<Func<ITextSplitter>>("Underlying"))).As<ITextSplitter>().SingleInstance();
            
            builder.RegisterType<ProcessingPipeline>().As<IProcessingPipeline>();
            builder.RegisterType<TestingClient>().As<ITestingClient>();
            builder.RegisterType<TrainingClient>().As<ITrainingClient>();
            builder.RegisterInstance(TaskPoolScheduler.Default).As<IScheduler>();

            builder.RegisterType<SessionContext>().As<ISessionContext>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ContextWordsDataLoader>().As<IContextWordsHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ContextSentenceRepairHandler>().As<IContextSentenceRepairHandler>().InstancePerLifetimeScope();
            builder.RegisterAggregateService<IClientContext>();
        }
    }
}
