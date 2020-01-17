using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using Wikiled.Common.Extensions;
using Wikiled.Common.Utilities.Serialization;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Pipeline.Persistency
{
    public class SimplePipelinePersistency : IPipelinePersistency
    {
        private CsvWriter csvDataOut;

        private IJsonStreamingWriter resultsWriter;

        private INRCDictionary dictionary;

        private StreamWriter streamWriter;

        private readonly IJsonStreamingWriterFactory resultsWriterFactory;

        public SimplePipelinePersistency(INRCDictionary dictionary, IJsonStreamingWriterFactory resultsWriterFactory)
        {
            this.dictionary = dictionary;
            this.resultsWriterFactory = resultsWriterFactory;
        }

        public bool ExtractStyle { get; set; }

        public bool Debug { get; set; }

        public void Start(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path.EnsureDirectoryExistence();
            streamWriter = new StreamWriter(Path.Combine(path, "results.csv"), false);
            csvDataOut = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            resultsWriter = resultsWriterFactory.CreateJson(Path.Combine(path, "result.json"));
            SetupHeader();
            csvDataOut.NextRecord();
        }

        private void SetupHeader()
        {
            csvDataOut.WriteField("Id");
            csvDataOut.WriteField("Date");
            csvDataOut.WriteField("Original");
            csvDataOut.WriteField("Calculated");
            csvDataOut.WriteField("TotalSentimentWords");
        }


        public void Save(ProcessingContext context)
        {
            var vector = new SentimentVector();
            if (ExtractStyle)
            {
                foreach (var word in context.Processed.Words)
                {
                    vector.ExtractData(dictionary.FindRecord(word));
                }
            }

            lock (csvDataOut)
            {
                csvDataOut.WriteField(context.Original.Id);
                csvDataOut.WriteField(context.Original.DocumentTime);
                csvDataOut.WriteField(context.Original.Stars);
                csvDataOut.WriteField(context.Adjustment.Rating.StarsRating);
                csvDataOut.WriteField(context.Review.GetAllSentiments().Length);
                if (ExtractStyle)
                {
                    csvDataOut.WriteField(vector.Anger);
                    csvDataOut.WriteField(vector.Anticipation);
                    csvDataOut.WriteField(vector.Disgust);
                    csvDataOut.WriteField(vector.Fear);
                    csvDataOut.WriteField(vector.Joy);
                    csvDataOut.WriteField(vector.Sadness);
                    csvDataOut.WriteField(vector.Surprise);
                    csvDataOut.WriteField(vector.Trust);
                    csvDataOut.WriteField(vector.Total);
                }

                csvDataOut.NextRecord();
                csvDataOut.Flush();
            }

            if (Debug)
            {
                resultsWriter.WriteObject(context.Processed);
            }
        }

        public void Dispose()
        {
            csvDataOut?.Dispose();
            resultsWriter?.Dispose();
            streamWriter?.Dispose();
        }
    }
}
