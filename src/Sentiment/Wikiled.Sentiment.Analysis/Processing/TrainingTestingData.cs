namespace Wikiled.Sentiment.Analysis.Processing
{
    public class TrainingTestingData
    {
        public TrainingTestingData()
        {
            Training = new ProcessingData();
            Testing = new ProcessingData();
        }

        public IProcessingData GetAll()
        {
            ProcessingData data = new ProcessingData();
            Training.Populate(data);
            Testing.Populate(data);
            return data;
        }

        public ProcessingData Training { get; set; }

        public ProcessingData Testing { get; set; }
    }
}