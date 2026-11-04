using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab05Wiring : ILab05Wiring
{
    private readonly Lab02Wiring _lab02 = new();

    public IItemStore CreateStore() => _lab02.CreateStore();

    public IItem CreateSample(int seed) => _lab02.CreateSample(seed);

    public Type CollectionType => typeof(Inventory);

    public Type GenericDefinition => typeof(RecentlyUsed<>);

    public IItem At(IItemStore store, int index) => GetInventory(store)[index];

    public IItem? ById(IItemStore store, string id) => GetInventory(store)[id];

    public IReadOnlyList<IItem> EachInOrder(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        var items = new List<IItem>();
        foreach (var item in GetInventory(store))
            items.Add(item);

        return items;
    }

    public IEnumerable<IItem> Matching(IItemStore store, string text)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(text);

        foreach (var item in GetInventory(store))
        {
            if (item.DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase))
                yield return item;
        }
    }

    public IReadOnlyList<T> Recent<T>(int capacity, params T[] pushed)
    {
        ArgumentNullException.ThrowIfNull(pushed);

        var recent = new RecentlyUsed<T>(capacity);
        foreach (var value in pushed)
            recent.Push(value);

        return recent.Values;
    }

    private static Inventory GetInventory(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        return store as Inventory
               ?? throw new ArgumentException("The store is not an Inventory.", nameof(store));
    }
}