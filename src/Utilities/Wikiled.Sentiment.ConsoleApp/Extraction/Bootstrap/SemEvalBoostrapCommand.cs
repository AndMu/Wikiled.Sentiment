using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data;
using Wikiled.Text.Analysis.Twitter;

namespace Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap
{
    /// <summary>
    ///     semboot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train
    /// </summary>
    [Description("Bootstrap training dataset from SemEval-2017")]
    public class SemEvalBoostrapCommand : BaseBoostrapCommand
    {
        private readonly MessageCleanup cleanup = new MessageCleanup();

        private static readonly ILogger log = ApplicationLogging.CreateLogger<SemEvalBoostrapCommand>();

        private Dictionary<string, string> exist;

        public override string Name { get; } = "semboot";

        protected override Task Execute(CancellationToken token)
        {
            exist = new Dictionary<string, string>();
            return base.Execute(token);
        }

        protected override IObservable<EvalData> GetDataPacket(string file)
        {
            return GetDataPacketEnum(file).ToObservable();
        }

        protected override void SaveResult(EvalData[] subscriptionMessage)
        {
            using (StreamWriter streamWrite = new StreamWriter(Destination, false, Encoding.UTF8))
            {
                foreach (EvalData item in subscriptionMessage)
                {
                    streamWrite.WriteLine($"{item.Id}\t{item.CalculatedPositivity.Value.ToString().ToLower()}\t{item.Text}");
                    streamWrite.Flush();
                }
            }
        }

        private IEnumerable<EvalData> GetDataPacketEnum(string file)
        {
            using (StreamReader streamRead = new StreamReader(file))
            {
                string line;
                while ((line = streamRead.ReadLine()) != null)
                {
                    long? id = null;
                    PositivityType? positivity = null;
                    string[] blocks = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (blocks.Length < 3)
                    {
                        log.LogError($"Error: {line}");
                        yield break;
                    }

                    if (long.TryParse(blocks[0], out long idValue))
                    {
                        id = idValue;
                    }

                    string textBlock = blocks[blocks.Length - 1];
                    string sentiment = blocks[blocks.Length - 2];
                    if (sentiment == "positive")
                    {
                        positivity = PositivityType.Positive;
                    }
                    else if (sentiment == "negative")
                    {
                        positivity = PositivityType.Negative;
                    }
                    else if (sentiment == "neutral")
                    {
                        positivity = PositivityType.Neutral;
                    }
                    else
                    {
                        if (int.TryParse(sentiment, out int value))
                        {
                            positivity = value > 0 ? PositivityType.Positive : value < 0 ? PositivityType.Negative : PositivityType.Neutral;
                        }
                    }

                    if (textBlock[0] == '\"' &&
                        textBlock[textBlock.Length - 1] == '\"')
                    {
                        textBlock = textBlock.Substring(1, textBlock.Length - 2);
                    }

                    string text = cleanup.Cleanup(textBlock);
                    if (!exist.ContainsKey(text))
                    {
                        exist[text] = text;
                        yield return new EvalData(id.ToString(), positivity, text);
                    }
                }
            }
        }
    }
}
