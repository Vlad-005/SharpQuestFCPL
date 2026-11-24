using Capstone.Core.Models;
using Capstone.Core.Wiring;
using SharpQuest.Contracts;

namespace Capstone.Tests;

public class Lab06Tests
{
    private readonly Lab06Wiring _wiring = new();

    [Fact]
    public void Subscribe_announces_the_item_added_after_it_is_in_the_store()
    {
        var inventory = new Inventory();
        var announced = new List<IItem>();
        var unsubscribe = _wiring.Subscribe(inventory, item =>
        {
            announced.Add(item);
            Assert.Same(item, inventory.FindById(item.Id));
        });

        var item = _wiring.CreateSample(1);
        inventory.Add(item);

        Assert.Equal([item], announced);
        unsubscribe();
    }

    [Fact]
    public void Unsubscribe_detaches_only_one_subscription()
    {
        var inventory = new Inventory();
        var firstCalls = 0;
        var secondCalls = 0;
        var unsubscribeFirst = _wiring.Subscribe(inventory, _ => firstCalls++);
        _wiring.Subscribe(inventory, _ => secondCalls++);

        unsubscribeFirst();
        inventory.Add(_wiring.CreateSample(1));

        Assert.Equal(0, firstCalls);
        Assert.Equal(1, secondCalls);
    }

    [Fact]
    public void A_duplicate_or_null_add_announces_nothing()
    {
        var inventory = new Inventory();
        var calls = 0;
        _wiring.Subscribe(inventory, _ => calls++);
        var item = _wiring.CreateSample(1);
        inventory.Add(item);

        Assert.Throws<ArgumentException>(() => inventory.Add(item));
        Assert.Throws<ArgumentNullException>(() => inventory.Add(null!));
        Assert.Equal(1, calls);
    }

    [Fact]
    public void Adding_with_no_subscribers_does_not_throw()
    {
        var inventory = new Inventory();
        var exception = Record.Exception(() => inventory.Add(_wiring.CreateSample(1)));
        Assert.Null(exception);
    }

    [Fact]
    public void Where_map_and_foreach_preserve_store_order()
    {
        var inventory = new Inventory();
        inventory.Add(_wiring.CreateSample(1));
        inventory.Add(_wiring.CreateSample(2));
        inventory.Add(_wiring.CreateSample(3));

        var ids = _wiring.Where(inventory, item => item.Id.Contains('1')).Select(item => item.Id);
        var descriptions = _wiring.Map(inventory, item => item.DisplayName);
        var visited = new List<string>();
        _wiring.ForEach(inventory, item => visited.Add(item.Id));

        Assert.Equal(["sample-1"], ids);
        Assert.Equal(inventory.All().Select(item => item.DisplayName), descriptions);
        Assert.Equal(inventory.All().Select(item => item.Id), visited);
    }

    [Fact]
    public void Function_arguments_reject_nulls()
    {
        var inventory = new Inventory();
        Assert.Throws<ArgumentNullException>(() => _wiring.Where(inventory, null!));
        Assert.Throws<ArgumentNullException>(() => _wiring.Map(inventory, null!));
        Assert.Throws<ArgumentNullException>(() => _wiring.ForEach(inventory, null!));
        Assert.Throws<ArgumentNullException>(() => _wiring.Subscribe(inventory, null!));
    }
}
