using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab06Wiring : ILab06Wiring
{
    private readonly Lab02Wiring _lab02 = new();

    public IItemStore CreateStore() => _lab02.CreateStore();

    public IItem CreateSample(int seed) => _lab02.CreateSample(seed);

    public Type PublisherType => typeof(Inventory);

    public Action Subscribe(IItemStore store, Action<IItem> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var inventory = GetInventory(store);
        inventory.ItemAdded += handler;
        return () => inventory.ItemAdded -= handler;
    }

    public IReadOnlyList<IItem> Where(IItemStore store, Func<IItem, bool> keep)
    {
        ArgumentNullException.ThrowIfNull(keep);

        var kept = new List<IItem>();
        foreach (var item in GetInventory(store))
        {
            if (keep(item))
                kept.Add(item);
        }

        return kept;
    }

    public IReadOnlyList<string> Map(IItemStore store, Func<IItem, string> describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        var descriptions = new List<string>();
        foreach (var item in GetInventory(store))
            descriptions.Add(describe(item));

        return descriptions;
    }

    public void ForEach(IItemStore store, Action<IItem> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in GetInventory(store))
            action(item);
    }

    private static Inventory GetInventory(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        return store as Inventory
               ?? throw new ArgumentException("The store is not an Inventory.", nameof(store));
    }
}