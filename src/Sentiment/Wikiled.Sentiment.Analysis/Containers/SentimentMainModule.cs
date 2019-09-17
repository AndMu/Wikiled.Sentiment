using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Wikiled.Common.Utilities.Modules;
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
    public class SentimentMainModule : IModule
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.RegisterModule<DefaultNlpModule>();

            services.AddTransient<IDataLoader, DataLoader>();
            services.AddTransient<ISessionContainer, SessionContainer>();
            services.AddSingleton<ILexiconConfiguration, LexiconConfiguration>();
            services.AddSingleton<InquirerManager>().As<InquirerManager, IInquirerManager>(item => item.Load());
            services.AddTransient<IParsedReviewManager, ParsedReviewManager>();

            services.AddSingleton<ISentenceRepairHandler, SentenceRepairHandler>();
            services.AddSingleton<IExtendedWords, ExtendedWords>();

            services.AddSingleton<IMemoryCache>(c => new MemoryCache(new MemoryCacheOptions()));

            services.AddSingleton<IWordFactory, WordOccurenceFactory>();

            var parallel = Environment.ProcessorCount;
            if (parallel > 30)
            {
                parallel = 30;
            }

            builder.RegisterType<WordsHandler>().As<IWordsHandler>().SingleInstance().OnActivating(item => item.Instance.Load());
            services.AddTransient<IAspectSerializer, AspectSerializer>();
            builder.Register(item => new QueueTextSplitter(parallel, item.ResolveNamed<Func<ITextSplitter>>("Underlying"))).As<ITextSplitter>().SingleInstance();

            services.AddTransient<IProcessingPipeline, ProcessingPipeline>();
            services.AddTransient<ITestingClient, TestingClient>();
            services.AddTransient<ITrainingClient, TrainingClient>();

            builder.RegisterType<SessionContext>().As<ISessionContext>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ContextWordsDataLoader>().As<IContextWordsHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ContextSentenceRepairHandler>().As<IContextSentenceRepairHandler>().InstancePerLifetimeScope();
            builder.RegisterAggregateService<IClientContext>();

            return services;
        }
    }
}
