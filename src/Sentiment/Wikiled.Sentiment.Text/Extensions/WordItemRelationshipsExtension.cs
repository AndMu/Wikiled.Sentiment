using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class WordItemRelationshipsExtension
    {
        public static IWordItem GetNextByIndex(this IWordItemRelationships current, int index)
        {
            IWordItem currentItem = current.Owner;
            while (index != 0)
            {
                if (currentItem == null)
                {
                    return null;
                }

                currentItem = index < 0 ? currentItem.Relationship.Previous : currentItem.Relationship.Next;
                index += index < 0 ? 1 : -1;
            }

            return currentItem;
        }
    }
}
