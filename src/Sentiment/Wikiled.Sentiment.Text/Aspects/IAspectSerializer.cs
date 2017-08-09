using System.Xml.Linq;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public interface IAspectSerializer
    {
        XDocument Serialize(IMainAspectHandler aspectHandler);

        XDocument Serialize(IWordItem[] aspects, IWordItem[] attributes);

        XDocument Serialize(IAspectDectector dectector);

        IAspectDectector Deserialize(XDocument document);
    }
}