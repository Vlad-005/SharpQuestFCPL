using Capstone.Core.Models;
using Capstone.Core.Wiring;

namespace Capstone.Tests;

public class Lab05Tests
{
    private readonly Lab05Wiring _wiring = new();

    [Fact]
    public void Inventory_indexers_return_items_by_position_and_id()
    {
        var first = _wiring.CreateSample(1);
        var second = _wiring.CreateSample(2);
        var inventory = new Inventory();
        inventory.Add(first);
        inventory.Add(second);

        Assert.Same(first, inventory[0]);
        Assert.Same(second, inventory[second.Id]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = inventory[2]);
    }

    [Fact]
    public void Inventory_can_be_enumerated_in_insertion_order()
    {
        var inventory = new Inventory();
        inventory.Add(_wiring.CreateSample(3));
        inventory.Add(_wiring.CreateSample(1));

        Assert.Equal(inventory.All().Select(item => item.Id), inventory.Select(item => item.Id));
    }

    [Fact]
    public void Matching_is_lazy_and_case_insensitive()
    {
        var inventory = new Inventory();
        inventory.Add(_wiring.CreateSample(1));
        var matches = _wiring.Matching(inventory, "SAMPLE");
        inventory.Add(_wiring.CreateSample(2));

        Assert.Equal(2, matches.Count());
    }

    [Fact]
    public void Recently_used_keeps_unique_values_newest_first_with_a_capacity()
    {
        var recent = new RecentlyUsed<string>(3);
        recent.Push("a");
        recent.Push("b");
        recent.Push("c");
        recent.Push("a");

        Assert.Equal(new[] { "a", "c", "b" }, recent.Values);
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecentlyUsed<int>(0));
    }
}