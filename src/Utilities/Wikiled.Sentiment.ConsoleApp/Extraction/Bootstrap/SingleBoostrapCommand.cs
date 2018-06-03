using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data;

namespace Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap
{
    /// <summary>
    ///     boot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train
    /// </summary>
    [Description("Bootstrap training dataset from single file")]
    public class SingleBoostrapCommand : ImdbBoostrapCommand
    {
        private int id;

        public override string Name { get; } = "boot";

        protected override IEnumerable<EvalData> GetDataPacket(string path)
        {
            path = path.ToLower();
            if (path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                var data = new DataLoader().Load(path);
                foreach (var processingData in data.Neutral.Concat(data.Negative).Concat(data.Positive))
                {
                    yield return new EvalData(processingData.Id, null, processingData.Text);
                }
            }
            else
            {
                foreach (var line in File.ReadLines(path).Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    id++;
                    yield return new EvalData(id.ToString(), null, line);
                }
            }
        }
    }
}