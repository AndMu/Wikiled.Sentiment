using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic;
using Wikiled.Sentiment.Analysis.Amazon;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    /// <summary>
    /// amazonredis -Source=E:\Data\Amazon\Electronics.txt -Category=Electronics -Port=6370
    /// amazonredis -Source=E:\Data\Amazon\Amazon_Instant_Video.txt -Category=Video -Port=6370
    /// amazonredis -Source=E:\Data\Amazon\Video_Games.txt -Category=Games -Port=6370
    /// amazonredis -Source=E:\Data\Amazon\Toys_&_Games.txt -Category=Toys-Port=6370
    /// </summary>
    public class AmazonParserRedis : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        public string Source { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public ProductCategory Category { get; set; }

        public int? Limit { get; set; }

        public override void Execute()
        {
            AmazonParserLogic parser = new AmazonParserLogic();
            parser.Category = Category;
            log.Info($"Starting {Category}");
            if (Limit.HasValue)
            {
                log.Info($"Limited to {Limit}");
            }

            using (RedisLink redis = new RedisLink("Wikiled", new RedisMultiplexer(new RedisConfiguration("localhost", Port))))
            {
                redis.Open();
                AmazonRepository repository = new AmazonRepository(redis);
                int total = 0;
                List<Task> tasks = new List<Task>();
                foreach (var review in parser.Parse(Source))
                {
                    total++;
                    if (total > Limit)
                    {
                        break;
                    }

                    if (total % 1000 == 0)
                    {
                        log.Info("Processing: {0}", total);
                        Task.WaitAll(tasks.ToArray());
                        tasks.Clear();
                    }

                    tasks.Add(repository.Save(review));
                }

                Task.WaitAll(tasks.ToArray());
                redis.Close();
                log.Info("Completed...");
                Console.ReadLine();
            }
        }
    }
}
