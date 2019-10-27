using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Pipeline.Persistency;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Containers;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SentimentMainModule : IModule
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.RegisterModule<DefaultNlpModule>();

            services.AddSingleton<IDataLoader, DataLoader>();
            services.AddTransient<IPipelinePersistency, SimplePipelinePersistency>();
            
            services.AddTransient<ISessionContainer, SessionContainer>();
            services.AddSingleton<ILexiconConfiguration, LexiconConfiguration>();
            services.AddSingleton<InquirerManager>().AsSingleton<IInquirerManager, InquirerManager>(item => item.Load());
            services.AddTransient<IParsedReviewManager, ParsedReviewManager>();

            services.AddScoped<Func<Document, IParsedReviewManager>>(ctx =>
                                                                            document => new ParsedReviewManager(
                                                                                ctx.GetService<IContextWordsHandler>(),
                                                                                ctx.GetService<IWordFactory>(),
                                                                                ctx.GetService<INRCDictionary>(),
                                                                                document));

            services.AddSingleton<ISentenceRepairHandler, SentenceRepairHandler>();
            services.AddSingleton<IExtendedWords, ExtendedWords>();

            services.AddSingleton<IMemoryCache>(c => new MemoryCache(new MemoryCacheOptions()));

            services.AddTransient<IWordFactory, WordOccurenceFactory>();

            var parallel = Environment.ProcessorCount;
            if (parallel > 30)
            {
                parallel = 30;
            }

            services.AddSingleton<WordsHandler>().AsSingleton<IWordsHandler, WordsHandler>(item => item.Load());
            services.AddTransient<IAspectSerializer, AspectSerializer>();

            services.AddSingleton<ITextSplitter>(
                item => new QueueTextSplitter(
                    item.GetService<ILogger<QueueTextSplitter>>(),
                    parallel,
                    item.GetService<Func<ITextSplitter>>("underlying")))
                    .AddFactory<ITextSplitter, ITextSplitter>();

            services.AddTransient<IProcessingPipeline, ProcessingPipeline>();
            services.AddTransient<ITestingClient, TestingClient>().AddFactory<ITestingClient, TestingClient>();
            services.AddTransient<ITrainingClient, TrainingClient>().AddFactory<ITestingClient, TestingClient>();

            services.AddScoped<SessionContext>().As<ISessionContext, SessionContext>();
            services.AddScoped<IContextWordsHandler, ContextWordsDataLoader>();
            services.AddScoped<IContextSentenceRepairHandler, ContextSentenceRepairHandler>();
            services.AddScoped<IClientContext, ClientContext>();

            return services;
        }
    }
}
