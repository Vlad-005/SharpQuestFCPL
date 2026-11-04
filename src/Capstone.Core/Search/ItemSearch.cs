using SharpQuest.Contracts;

namespace Capstone.Core.Search;

public static class ItemSearch
{
    public static IReadOnlyList<IItem> Find(IItemStore store, string query)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(query);

        var matcher = SearchMatcher.Create();
        return
        [
            .. store.All().Where(item => matcher.Matches(SearchText(item), query))
        ];
    }

    private static string SearchText(IItem item) => item is Models.Item inventoryItem
        ? $"{inventoryItem.DisplayName} {inventoryItem.Category} Level {inventoryItem.RequiredLevel}"
        : item.DisplayName;
}