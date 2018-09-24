using System;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;
using Wikiled.Text.Analysis.Words;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Analysis.Processing.Containers
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LexiconConfiguration>().As<ILexiconConfiguration>().SingleInstance();
            builder.RegisterType<BasicEnglishDictionary>().As<IWordsDictionary>().SingleInstance();
            builder.RegisterType<InquirerManager>().As<IInquirerManager>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<NRCDictionary>().As<INRCDictionary>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<ParsedReviewManager>().As<IParsedReviewManager>();

            builder.RegisterType<SentenceTokenizerFactory>().As<ISentenceTokenizerFactory>().SingleInstance();
            builder.RegisterType<NaivePOSTagger>().As<IPOSTagger>().SingleInstance();
            builder.RegisterType<BNCList>().As<IPosTagResolver>().As<IWordFrequencyList>().SingleInstance();
            builder.Register(c => WordTypeResolver.Instance).As<IWordTypeResolver>().SingleInstance();

            builder.RegisterType<RawWordExtractor>().As<IRawTextExtractor>().SingleInstance();
            builder.Register(c => new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>().SingleInstance();

            builder.RegisterType<WordOccurenceFactory>().As<IWordFactory>();

            int parallel = Environment.ProcessorCount / 2;
            builder.RegisterType<WordsDataLoader>().As<IWordsHandler>().SingleInstance().OnActivating(item => item.Instance.Load());
            builder.RegisterType<AspectSerializer>().As<IAspectSerializer>().SingleInstance();
            builder.Register(item => new QueueTextSplitter(parallel, item.ResolveNamed<Func<ITextSplitter>>("Underlying"))).As<ITextSplitter>().SingleInstance();
        }
    }
}
