using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data;

namespace Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap
{
    /// <summary>
    ///     boot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    [Description("Bootstrap training dataset from single file")]
    public class SingleBoostrapCommand : ImdbBoostrapCommand
    {
        private int id = 0;

        public override string Name { get; } = "boot";

        protected override IEnumerable<EvalData> GetDataPacket(string path)
        {
            path = path.ToLower();
            foreach (var line in File.ReadLines(path).Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                id++;
                yield return new EvalData(id.ToString(), null, line);
            }
        }
    }
}