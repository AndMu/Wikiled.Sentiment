namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class StopWordItemPipeline : WordItemFilterOutPipeline
    {
        public StopWordItemPipeline() 
            : base(item => item.IsStopWord)
        {
        }
    }
}
