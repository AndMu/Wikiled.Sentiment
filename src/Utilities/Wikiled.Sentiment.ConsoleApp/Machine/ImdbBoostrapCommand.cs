using System.Collections.Generic;
using System.IO;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.ConsoleApp.Machine.Data;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    ///     bootimdb -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    public class ImdbBoostrapCommand : BaseBoostrapCommand
    {
        private string negativeResult;

        private string positiveResult;

        protected override IEnumerable<EvalData> GetDataPacket(string path)
        {
            path = path.ToLower();
            PositivityType? positivity = null;
            var id = System.IO.Path.GetFileNameWithoutExtension(path);
            var text = File.ReadAllText(path);
            if (path.Contains(@"\pos"))
            {
                positivity = PositivityType.Positive;
            }
            else if (path.Contains(@"\neg"))
            {
                positivity = PositivityType.Negative;
            }

            yield return new EvalData(id, positivity, text);
        }

        protected override void SaveResult(EvalData[] subscriptionMessage)
        {
            positiveResult = System.IO.Path.Combine(Destination, "pos");
            negativeResult = System.IO.Path.Combine(Destination, "neg");
            if (Directory.Exists(negativeResult))
            {
                Directory.Delete(negativeResult, true);
            }

            if (Directory.Exists(positiveResult))
            {
                Directory.Delete(positiveResult, true);
            }

            negativeResult.EnsureDirectoryExistence();
            positiveResult.EnsureDirectoryExistence();

            foreach (var item in subscriptionMessage)
            {
                if (item.CalculatedPositivity == PositivityType.Positive)
                {
                    File.WriteAllText(System.IO.Path.Combine(positiveResult, item.Id + ".txt"), item.Text);
                }

                if (item.CalculatedPositivity == PositivityType.Negative)
                {
                    File.WriteAllText(System.IO.Path.Combine(negativeResult, item.Id + ".txt"), item.Text);
                }
            }
        }
    }
}