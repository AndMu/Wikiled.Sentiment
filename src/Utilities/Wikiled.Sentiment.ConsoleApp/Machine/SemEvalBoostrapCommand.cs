using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.ConsoleApp.Machine.Data;
using Wikiled.Text.Analysis.Twitter;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    ///     semboot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    [Description("Bootstrap training dataset from SemEval-2017")]
    public class SemEvalBoostrapCommand : BaseBoostrapCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly MessageCleanup cleanup = new MessageCleanup();

        private Dictionary<string, string> exist;

        public override string Name { get; } = "semboot";

        public override void Execute()
        {
            exist = new Dictionary<string, string>();
            base.Execute();
        }

        protected override IEnumerable<EvalData> GetDataPacket(string file)
        {
            using (var streamRead = new StreamReader(file))
            {
                string line;
                while ((line = streamRead.ReadLine()) != null)
                {
                    long? id = null;
                    PositivityType? positivity = null;
                    var blocks = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (blocks.Length < 3)
                    {
                        log.Error($"Error: {line}");
                        yield break;
                    }

                    long idValue;
                    if (long.TryParse(blocks[0], out idValue))
                    {
                        id = idValue;
                    }

                    var textBlock = blocks[blocks.Length - 1];
                    var sentiment = blocks[blocks.Length - 2];
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
                        int value;
                        if (int.TryParse(sentiment, out value))
                        {
                            positivity = value > 0 ? PositivityType.Positive : value < 0 ? PositivityType.Negative : PositivityType.Neutral;
                        }
                    }

                    if (textBlock[0] == '\"' &&
                        textBlock[textBlock.Length - 1] == '\"')
                    {
                        textBlock = textBlock.Substring(1, textBlock.Length - 2);
                    }

                    var text = cleanup.Cleanup(textBlock);
                    if (!exist.ContainsKey(text))
                    {
                        exist[text] = text;
                        yield return new EvalData(id.ToString(), positivity, text);
                    }
                }
            }
        }

        protected override void SaveResult(EvalData[] subscriptionMessage)
        {
            using (var streamWrite = new StreamWriter(Destination, false, Encoding.UTF8))
            {
                foreach (var item in subscriptionMessage)
                {
                    streamWrite.WriteLine($"{item.Id}\t{item.CalculatedPositivity.Value.ToString().ToLower()}\t{item.Text}");
                    streamWrite.Flush();
                }
            }
        }
    }
}
