using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab07Wiring : ILab07Wiring
{
    private readonly Lab02Wiring _lab02 = new();

    public IItemStore CreateStore() => _lab02.CreateStore();

    public IItem CreateSample(int seed) => _lab02.CreateSample(seed);

    public IEnumerable<string> Names(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        var inventory = GetInventory(store);
        return inventory.Select(item => item.DisplayName);
    }

    public IReadOnlyList<IItem> Sorted(IEnumerable<IItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return items
            .OrderBy(item => item.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyList<IItem> Page(IEnumerable<IItem> items, int skip, int take)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(skip);
        ArgumentOutOfRangeException.ThrowIfNegative(take);

        return Sorted(items)
            .Skip(skip)
            .Take(take)
            .ToArray();
    }

    public IReadOnlyList<IItem> Longest(IEnumerable<IItem> items, int n)
        => items.Longest(n);

    public Type ExtensionHost => typeof(InventoryQueryExtensions);

    public StoreStats Summary(IEnumerable<IItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var materialized = items.ToArray();
        if (materialized.Length == 0)
            return new StoreStats(0, 0, null, 0);

        var longest = materialized.Longest(1).First();
        var distinctInitials = materialized
            .Select(item => item.DisplayName[..1])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
        var averageNameLength = materialized.Average(item => item.DisplayName.Length);

        return new StoreStats(
            materialized.Length,
            distinctInitials,
            longest.DisplayName,
            averageNameLength);
    }

    private static Inventory GetInventory(IItemStore store)
        => store as Inventory
           ?? throw new ArgumentException("The store is not an Inventory.", nameof(store));
}

public static class InventoryQueryExtensions
{
    public static IReadOnlyList<IItem> Longest(this IEnumerable<IItem> items, int n)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(n);

        return items
            .OrderByDescending(item => item.DisplayName.Length)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .Take(n)
            .ToArray();
    }
}
