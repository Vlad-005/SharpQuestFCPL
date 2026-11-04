using Capstone.Core.Models;
using Capstone.Core.Wiring;
using SharpQuest.Contracts;

namespace Capstone.Tests;

public class Lab04Tests
{
    private readonly Lab04Wiring _wiring = new();

    [Fact]
    public void Created_kinds_share_the_item_base_and_describe_differently()
    {
        var weapon = _wiring.CreateOfKind(0, 5);
        var armor = _wiring.CreateOfKind(1, 5);

        Assert.True(weapon is Item);
        Assert.True(armor is Item);
        Assert.NotEqual(_wiring.Describe(weapon), _wiring.Describe(armor));
    }

    [Fact]
    public void Describe_all_preserves_store_order()
    {
        var store = _wiring.CreateStore();
        store.Add(_wiring.CreateOfKind(1, 2));
        store.Add(_wiring.CreateOfKind(0, 1));

        Assert.Equal(store.All().Select(_wiring.Describe), _wiring.DescribeAll(store));
    }

    [Fact]
    public void Classify_handles_known_foreign_and_null_items()
    {
        Assert.Equal("Weapon", _wiring.Classify(_wiring.CreateOfKind(0, 1)));
        Assert.Equal("Armor", _wiring.Classify(_wiring.CreateOfKind(1, 1)));
        Assert.Equal("Consumable", _wiring.Classify(_wiring.CreateOfKind(2, 1)));
        Assert.Equal("Other item", _wiring.Classify(new ForeignItem("foreign", "Unknown")));
        Assert.Throws<ArgumentNullException>(() => _wiring.Classify(null!));
    }

    [Fact]
    public void Try_find_by_name_is_exact_and_sets_out_value()
    {
        var store = _wiring.CreateStore();
        var item = _wiring.CreateOfKind(0, 1);
        store.Add(item);

        Assert.True(_wiring.TryFindByName(store, item.DisplayName, out var found));
        Assert.Same(item, found);
        Assert.False(_wiring.TryFindByName(store, item.DisplayName.ToUpperInvariant(), out found));
        Assert.Null(found);
    }

    [Fact]
    public void Add_all_supports_zero_or_more_items_and_keeps_order()
    {
        var store = _wiring.CreateStore();
        Assert.Equal(0, _wiring.AddAll(store));

        var first = _wiring.CreateOfKind(0, 1);
        var second = _wiring.CreateOfKind(1, 1);

        Assert.Equal(2, _wiring.AddAll(store, first, second));
        string[] expectedIds = [first.Id, second.Id];
        Assert.Equal(expectedIds, store.All().Select(item => item.Id));
    }
}