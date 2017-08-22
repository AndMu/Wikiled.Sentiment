using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Reflection;
using Wikiled.Text.Inquirer.Reflection.Data;

namespace Wikiled.Sentiment.Text.NLP.NRC
{
    public class SentimentVector : ISentimentDimension
    {
        private int anger;

        private int anticipation;

        private int disgust;

        private int fear;

        private int joy;

        private int sadness;

        private int surprise;

        private int total;

        private int totalSentiment;

        private int trust;

        [InfoField("Anger")]
        public int Anger => anger;

        [InfoField("Anticipation")]
        public int Anticipation => anticipation;

        [InfoField("Disgust")]
        public int Disgust => disgust;

        [InfoField("Fear")]
        public int Fear => fear;

        [InfoField("Joy")]
        public int Joy => joy;

        [InfoField("Sadness")]
        public int Sadness => sadness;

        public double? Sentiment { get; set; }

        [InfoField("Surprise")]
        public int Surprise => surprise;

        public int Total => total;

        public int TotalSum => Anger +
                               Anticipation +
                               Disgust +
                               Fear +
                               Joy +
                               Sadness +
                               Surprise +
                               Trust;

        [InfoField("Trust")]
        public int Trust => trust;

        public void Extract(IEnumerable<IWordItem> words)
        {
            Parallel.ForEach(words, AsyncSettings.DefaultParallel, word => ExtractData(NRCDictionary.Instance.FindRecord(word)));
        }

        public void Extract(WordEx[] words)
        {
            Parallel.ForEach(words, AsyncSettings.DefaultParallel, word => ExtractData(NRCDictionary.Instance.FindRecord(word)));
        }

        public IEnumerable<Tuple<ItemProbability<string>, ItemProbability<string>>> GetOccurencePairs()
        {
            yield return new Tuple<ItemProbability<string>, ItemProbability<string>>(
                new ItemProbability<string>("Joy") {Probability = Joy},
                new ItemProbability<string>("Sadness") {Probability = Sadness});
            yield return new Tuple<ItemProbability<string>, ItemProbability<string>>(
                new ItemProbability<string>("Anger") {Probability = Anger},
                new ItemProbability<string>("Fear") {Probability = Fear});
            yield return new Tuple<ItemProbability<string>, ItemProbability<string>>(
                new ItemProbability<string>("Trust") {Probability = Trust},
                new ItemProbability<string>("Disgust") {Probability = Disgust});
            yield return new Tuple<ItemProbability<string>, ItemProbability<string>>(
                new ItemProbability<string>("Anticipation") {Probability = Anticipation},
                new ItemProbability<string>("Surprise") {Probability = Surprise});
        }

        public IEnumerable<ItemProbability<string>> GetOccurences()
        {
            return GetOccurencesInternal();
        }

        public IEnumerable<ItemProbability<string>> GetProbabilities()
        {
            return GetOccurencesInternal(Total);
        }

        private void ExtractData(NRCRecord record)
        {
            Interlocked.Increment(ref total);
            if (record == null)
            {
                return;
            }

            bool added = false;
            if (record.IsAnger)
            {
                added = true;
                Interlocked.Increment(ref anger);
            }

            if (record.IsAnticipation)
            {
                added = true;
                Interlocked.Increment(ref anticipation);
            }

            if (record.IsDisgust)
            {
                added = true;
                Interlocked.Increment(ref disgust);
            }

            if (record.IsFear)
            {
                added = true;
                Interlocked.Increment(ref fear);
            }

            if (record.IsJoy)
            {
                added = true;
                Interlocked.Increment(ref joy);
            }

            if (record.IsSadness)
            {
                added = true;
                Interlocked.Increment(ref sadness);
            }

            if (record.IsSurprise)
            {
                added = true;
                Interlocked.Increment(ref surprise);
            }

            if (record.IsTrust)
            {
                added = true;
                Interlocked.Increment(ref trust);
            }

            if (added)
            {
                Interlocked.Increment(ref totalSentiment);
            }
        }

        private IEnumerable<ItemProbability<string>> GetOccurencesInternal(double divisor = 1)
        {
            List<ItemProbability<string>> list = new List<ItemProbability<string>>();
            list.Add(new ItemProbability<string>("Anger") {Probability = Anger / divisor});
            list.Add(new ItemProbability<string>("Anticipation") {Probability = Anticipation / divisor});
            list.Add(new ItemProbability<string>("Disgust") {Probability = Disgust / divisor});
            list.Add(new ItemProbability<string>("Fear") {Probability = Fear / divisor});
            list.Add(new ItemProbability<string>("Joy") {Probability = Joy / divisor});
            list.Add(new ItemProbability<string>("Sadness") {Probability = Sadness / divisor});
            list.Add(new ItemProbability<string>("Surprise") {Probability = Surprise / divisor});
            list.Add(new ItemProbability<string>("Trust") {Probability = Trust / divisor});
            return list.OrderBy(item => item.Data);
        }
    }
}
