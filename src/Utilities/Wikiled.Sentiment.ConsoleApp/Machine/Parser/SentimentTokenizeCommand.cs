using System;
using System.Threading.Tasks;
using NLog;
using Polly.Retry;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Cache;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Helpers;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.NLP.Stanford;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Parser.Cache;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Machine.Parser
{
    /// <summary>
    /// tokenize -Tagger=Stanford -Port=6370
    /// </summary>
    public class SentimentTokenizeCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        public POSTaggerType Tagger { get; set; }

        [Required]
        public int Port { get; set; }

        private readonly RetryPolicy retryPolicy = RetryHandler.Construct(true);

        public override void Execute()
        {
            try
            {
                retryPolicy.ExecuteAsync(ProcessRedis).Wait();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        private async Task ProcessRedis()
        {
            using (RedisLink manager = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration("localhost", Port))))
            {
                manager.Open();
                var cacheFactory = new RedisDocumentCacheFactory(manager);
                SplitterHelper helper = new SplitterHelper(cacheFactory, new ConfigurationHandler(), new StanfordFactory(cacheFactory));
                helper.Load();
                DocumentTokenizer tokenizer = new DocumentTokenizer(helper.Splitter);
                AmazonRepository repository = new AmazonRepository(manager);

                do
                {
                    var review = manager.Database.ListLeftPop("Wikiled:Analysis:Tokenize");
                    if (review.IsNull)
                    {
                        return;
                    }

                    log.Info("Processing review file: {0}", review);
                    var amazonReview = await repository.Load(review).ConfigureAwait(false);
                    var document = new Document(amazonReview.TextData.Text);
                    document.Stars = amazonReview.Data.Score;
                    document.Id = amazonReview.Id;
                    document.DocumentTime = amazonReview.Data.Date;
                    document.Author = amazonReview.User.Id;

                    SingleProcessingData item = new SingleProcessingData(document);
                    try
                    {
                        await tokenizer.Tokenize(item, false).ConfigureAwait(false);
                        if (tokenizer.Errors.Count == 0)
                        {
                            log.Info("No errors in {0} file parsing", review);
                        }
                    }
                    catch (ParsingException ex)
                    {
                        log.Error(ex);
                    }
                }
                while (true);
            }
        }
    }
}
