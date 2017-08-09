namespace Wikiled.Sentiment.Text.Aspects
{
    public interface IMainAspectHandlerFactory
    {
        IMainAspectHandler Construct();

        IAspectSerializer ConstructSerializer();
    }
}