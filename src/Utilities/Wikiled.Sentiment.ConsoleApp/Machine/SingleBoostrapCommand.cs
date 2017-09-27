using System.Collections.Generic;
using System.IO;
using Wikiled.Sentiment.ConsoleApp.Machine.Data;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    ///     boot -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    public class SingleBoostrapCommand : ImdbBoostrapCommand
    {
        private int id = 0;

        protected override IEnumerable<EvalData> GetDataPacket(string path)
        {
            path = path.ToLower();
            foreach (var line in File.ReadLines(path))
            {
                id++;
                yield return new EvalData(id.ToString(), null, line);
            }
        }
    }
}