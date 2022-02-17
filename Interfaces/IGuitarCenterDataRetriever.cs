using GuitarCenterGearFinder.Classes;

namespace GuitarCenterGearFinder.Interfaces
{
    public interface IGuitarCenterDataRetriever
    {
        IEnumerable<ListedItem> ListedItemsFound(string searchTerm);

        // Takes a list of items and removes any returned already existing
        IEnumerable<ListedItem> GetNewListedItems(string searchTerm);
    }
}
