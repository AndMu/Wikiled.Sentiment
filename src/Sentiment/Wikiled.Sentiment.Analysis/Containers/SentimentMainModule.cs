using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Common.Utilities.Resources.Config;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Analysis.Pipeline.Persistency;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Persistency;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.NLP.NER;
using Wikiled.Sentiment.Text.NLP.OpenNLP;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Containers;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Analysis.Containers
{
    public class SentimentMainModule : IModule
    {
        public POSTaggerType Tagger { get; set; } = POSTaggerType.SharpNLP;

        public bool UseNER { get; set; }

        public bool LocalConfig { get; set; }

        public string Root { get; set; }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.RegisterModule<DefaultNlpModule>();

            services.AddSingleton<IDataLoader, DataLoader>();
            services.AddTransient<IPipelinePersistency, SimplePipelinePersistency>();

            services.AddTransient<ISessionContainer, SessionContainer>();
            services.AddSingleton<LexiconConfigLoader>();
            if (LocalConfig)
            {
                services.AddSingleton(ctx => ctx.GetRequiredService<LexiconConfigLoader>().Load(Root));
            }

            services.AddSingleton<InquirerManager>().AsSingleton<IInquirerManager, InquirerManager>(item => item.Load());
            services.AddTransient<IParsedReviewManager, ParsedReviewManager>();

            services.AddTransient<Func<Document, IParsedReviewManager>>(ctx =>
                                                                            document => new ParsedReviewManager(
                                                                                ctx.GetService<IContextWordsHandler>(),
                                                                                ctx.GetService<IWordFactory>(),
                                                                                ctx.GetService<INRCDictionary>(),
                                                                                document));

            if (UseNER)
            {
                services.AddSingleton<INamedEntityRecognition, OpenNlpNamedEntityRecognition>();
            }
            else
            {
                services.AddSingleton<INamedEntityRecognition, NullNamedEntityRecognition>();
            }

            services.AddSingleton<ISentenceRepairHandler, SentenceRepairHandler>();
            services.AddSingleton<IExtendedWords, ExtendedWords>();

            services.AddSingleton<IMemoryCache>(c => new MemoryCache(new MemoryCacheOptions()));

            services.AddTransient<IWordFactory, WordOccurenceFactory>();

            services.AddSingleton<WordsHandler>().AsSingleton<IWordsHandler, WordsHandler>(item => item.Load());
            services.AddTransient<IAspectSerializer, AspectSerializer>();

            if (Tagger == POSTaggerType.SharpNLP)
            {
                services.AddTransient<OpenNLPTextSplitter>();
                services.AddTransient<Func<ITextSplitter>>(
                    ctx => () => new RecyclableTextSplitter(ctx.GetService<ILogger<RecyclableTextSplitter>>(),
                        ctx.GetService<OpenNLPTextSplitter>, new RecyclableConfig()),
                    "underlying");

                services.AddSingleton<ITextSplitter>(
                        item => new QueueTextSplitter(
                            item.GetService<ILogger<QueueTextSplitter>>(),
                            ParallelHelper.MaxParallel,
                            item.GetService<Func<ITextSplitter>>("underlying")))
                    .AddFactory<ITextSplitter, ITextSplitter>();
            }
            else
            {
                services.AddTransient<ITextSplitter, SimpleTextSplitter>();
            }

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
