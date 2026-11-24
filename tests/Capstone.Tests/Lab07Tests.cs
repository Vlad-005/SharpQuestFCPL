using Capstone.Core.Models;
using Capstone.Core.Wiring;
using SharpQuest.Contracts;

namespace Capstone.Tests;

public class Lab07Tests
{
    private readonly Lab07Wiring _wiring = new();

    [Fact]
    public void Names_is_lazy_and_uses_the_live_inventory()
    {
        var inventory = new Inventory();
        var names = _wiring.Names(inventory);

        inventory.Add(new Consumable("potion-1", "Healing Potion"));
        Assert.Equal(["Healing Potion"], names);

        inventory.Add(new Weapon("sword-1", "Iron Sword"));
        Assert.Equal(["Healing Potion", "Iron Sword"], names);
        Assert.Equal(names, names);
    }

    [Fact]
    public void Sorted_orders_names_without_changing_the_source()
    {
        IItem[] items =
        [
            new Consumable("c", "beta"),
            new Weapon("b", "Alpha"),
            new Armor("a", "alpha")
        ];

        var sorted = _wiring.Sorted(items);

        Assert.Equal(["a", "b", "c"], sorted.Select(item => item.Id));
        Assert.Equal(["c", "b", "a"], items.Select(item => item.Id));
    }

    [Fact]
    public void Page_uses_name_order_and_validates_arguments()
    {
        IItem[] items =
        [
            new Consumable("1", "Zulu"),
            new Weapon("2", "Alpha"),
            new Armor("3", "Mike")
        ];

        Assert.Equal(["Mike", "Zulu"], _wiring.Page(items, 1, 2).Select(item => item.DisplayName));
        Assert.Throws<ArgumentOutOfRangeException>(() => _wiring.Page(items, -1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => _wiring.Page(items, 0, -1));
    }

    [Fact]
    public void Longest_extension_orders_by_length_then_id()
    {
        IItem[] items =
        [
            new Consumable("b", "Two"),
            new Weapon("a", "Longest"),
            new Armor("c", "Tiny")
        ];

        var longest = items.Longest(2);

        Assert.Equal(["a", "c"], longest.Select(item => item.Id));
        Assert.Throws<ArgumentOutOfRangeException>(() => items.Longest(-1));
        Assert.Empty(items.Longest(0));
    }

    [Fact]
    public void Summary_handles_empty_and_full_sequences()
    {
        Assert.Equal(new StoreStats(0, 0, null, 0), _wiring.Summary(Array.Empty<IItem>()));

        IItem[] items =
        [
            new Consumable("1", "Alpha"),
            new Weapon("2", "amber"),
            new Armor("3", "Beta")
        ];
        var summary = _wiring.Summary(items);

        Assert.Equal(3, summary.Count);
        Assert.Equal(2, summary.DistinctInitials);
        Assert.Equal("Alpha", summary.LongestName);
        Assert.Equal(14.0 / 3.0, summary.AverageNameLength, 10);
    }
}
