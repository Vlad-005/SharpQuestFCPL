using Capstone.Core.Models;
using SharpQuest.Contracts;
using System.Collections.ObjectModel;

namespace Capstone.Core.Wiring;

public sealed class Lab04Wiring : ILab04Wiring
{
    private readonly Lab02Wiring _lab02 = new();

    private static readonly ReadOnlyCollection<Type> Kinds = Array.AsReadOnly(
    [
        typeof(Weapon),
        typeof(Armor),
        typeof(Consumable)
    ]);

    public IItemStore CreateStore() => _lab02.CreateStore();

    public Type BaseType => typeof(Item);

    public IReadOnlyList<Type> KindTypes => Kinds;

    public IItem CreateOfKind(int kindIndex, int seed)
    {
        if (kindIndex < 0 || kindIndex >= Kinds.Count)
            throw new ArgumentOutOfRangeException(nameof(kindIndex));
        if (seed is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(seed));

        var id = $"lab04-{kindIndex}-{seed}";
        var requiredLevel = (seed - 1) % 10;

        return kindIndex switch
        {
            0 => new Weapon(id, $"Iron Sword {seed}", requiredLevel),
            1 => new Armor(id, $"Iron Armor {seed}", requiredLevel),
            2 => new Consumable(id, $"Healing Potion {seed}", requiredLevel),
            _ => throw new ArgumentOutOfRangeException(nameof(kindIndex))
        };
    }

    public string Describe(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item is Item inventoryItem
            ? inventoryItem.Describe()
            : throw new ArgumentException("The item is not part of the inventory item hierarchy.", nameof(item));
    }

    public IReadOnlyList<string> DescribeAll(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        var descriptions = new List<string>();
        foreach (var item in store.All())
            descriptions.Add(Describe(item));

        return descriptions;
    }

    public string Classify(IItem item) => item switch
    {
        null => throw new ArgumentNullException(nameof(item)),
        Weapon => "Weapon",
        Armor => "Armor",
        Consumable => "Consumable",
        _ => "Other item"
    };

    public bool TryFindByName(IItemStore store, string name, out IItem? found)
    {
        ArgumentNullException.ThrowIfNull(store);
        found = null;
        if (string.IsNullOrWhiteSpace(name))
            return false;

        foreach (var item in store.All())
        {
            if (!string.Equals(item.DisplayName, name, StringComparison.Ordinal))
                continue;

            found = item;
            return true;
        }

        return false;
    }

    public int AddAll(IItemStore store, params IItem[] items)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(items);

        var added = 0;
        foreach (var item in items)
        {
            store.Add(item);
            added++;
        }

        return added;
    }
}