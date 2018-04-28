using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap.Data;

namespace Wikiled.Sentiment.ConsoleApp.Extraction.Bootstrap
{
    /// <summary>
    ///     bootimdb -Words=words.csv -Path="E:\DataSets\SemEval\All\out\ -Destination=c:\DataSets\SemEval\train.txt
    /// </summary>
    [Description("Bootstrap training dataset from IMDB")]
    public class ImdbBoostrapCommand : BaseBoostrapCommand
    {
        private string negativeResult;

        private string positiveResult;

        public override string Name { get; } = "bootimdb";

        protected override IEnumerable<EvalData> GetDataPacket(string path)
        {
            path = path.ToLower();
            PositivityType? positivity = null;
            var id = System.IO.Path.GetFileNameWithoutExtension(path);
            var text = File.ReadAllText(path);
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            if (path.Contains(@"/pos"))
            {
                positivity = PositivityType.Positive;
            }
            else if (path.Contains(@"/neg"))
            {
                positivity = PositivityType.Negative;
            }


            yield return new EvalData(id + $"_{positivity}", positivity, text);
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
                string place = item.CalculatedPositivity == PositivityType.Positive ? positiveResult : negativeResult;
                var file = System.IO.Path.Combine(place, item.Id + ".txt");
                SaveFile(file, item);
            }
        }

        private static void SaveFile(string file, EvalData item)
        {
            if (File.Exists(file))
            {
                throw new ApplicationException($"File already exist: {file}");
            }

            File.WriteAllText(file, item.Text);
        }
    }
}