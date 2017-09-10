using System.Collections.Generic;
using System.IO;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.ConsoleApp.Machine.Data;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    ///     bootimdb -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    public class ImdbBoostrapCommand : BoostrapCommand
    {
        public override int MinimumSentimentWords { get; } = 4;

        protected override IEnumerable<EvalData> GetDataPacket(string path)
        {
            path = path.ToLower();
            PositivityType? positivity = null;
            var id = System.IO.Path.GetFileNameWithoutExtension(path);
            var text = File.ReadAllText(path);
            if (path.Contains(@"\pos\"))
            {
                positivity = PositivityType.Positive;
            }
            else if (path.Contains(@"\neg\"))
            {
                positivity = PositivityType.Negative;
            }

            yield return new EvalData(id, positivity, text);
        }
    }
}
