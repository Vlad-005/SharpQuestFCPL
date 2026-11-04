using Capstone.Core.Models;
using Capstone.Core.Search;

namespace Capstone.Tests;

public class Lab03Tests
{
    [Theory]
    [InlineData("", "Healing Potion")]
    [InlineData("item-1", " ")]
    public void Item_rejects_blank_identity_or_display_name(string id, string displayName)
    {
        Assert.Throws<ArgumentException>(() => new Weapon(id, displayName));
    }

    [Fact]
    public void Item_rejects_a_negative_required_level()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Consumable("item-1", "Healing Potion", requiredLevel: -1));
    }

    [Fact]
    public void Inventory_rejects_duplicate_item_ids()
    {
        var inventory = new Inventory();
        inventory.Add(new Consumable("potion-1", "Healing Potion"));

        Assert.Throws<ArgumentException>(() => inventory.Add(new Consumable("potion-1", "Mana Potion")));
    }

    [Fact]
    public void Inventory_returns_items_in_added_order()
    {
        var inventory = new Inventory();
        inventory.Add(new Weapon("sword-1", "Iron Sword"));
        inventory.Add(new Consumable("potion-1", "Healing Potion"));

        string[] expectedIds = ["sword-1", "potion-1"];
        Assert.Equal(expectedIds, inventory.All().Select(item => item.Id));
    }

    [Fact]
    public void Inventory_all_returns_a_snapshot()
    {
        var inventory = new Inventory();
        inventory.Add(new Consumable("potion-1", "Healing Potion"));
        var snapshot = inventory.All();

        inventory.Add(new Weapon("sword-1", "Iron Sword"));

        Assert.Single(snapshot);
        Assert.Equal(2, inventory.Count);
    }

    [Fact]
    public void Inventory_stacks_matching_item_quantities()
    {
        var inventory = new Inventory();
        var potion = new Consumable("potion-1", "Healing Potion");

        inventory.AddToStack(potion, 2);
        inventory.AddToStack(new Consumable("potion-1", "Healing Potion"), 3);

        var stack = Assert.Single(inventory.Stacks);
        Assert.Equal("potion-1", stack.Item.Id);
        Assert.Equal(5, stack.Quantity);
    }

    [Fact]
    public void Inventory_rejects_non_positive_stack_quantities()
    {
        var inventory = new Inventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            inventory.AddToStack(new Consumable("potion-1", "Healing Potion"), 0));
    }

    [Fact]
    public void Search_matches_rpg_categories_and_level_requirements_without_reordering_items()
    {
        var inventory = new Inventory();
        inventory.Add(new Weapon("sword-1", "Iron Sword", 3));
        inventory.Add(new Consumable("potion-1", "Healing Potion", 1));
        inventory.Add(new Armor("armor-1", "Iron Armor", 3));

        var categoryMatches = ItemSearch.Find(inventory, "consumable");
        var levelMatches = ItemSearch.Find(inventory, "level 3");

        string[] expectedCategoryIds = ["potion-1"];
        string[] expectedLevelIds = ["sword-1", "armor-1"];
        Assert.Equal(expectedCategoryIds, categoryMatches.Select(item => item.Id));
        Assert.Equal(expectedLevelIds, levelMatches.Select(item => item.Id));
    }

    [Fact]
    public void Search_rejects_a_null_store()
    {
        Assert.Throws<ArgumentNullException>(() => ItemSearch.Find(null!, "potion"));
    }

    [Fact]
    public void Search_does_not_change_the_inventory()
    {
        var inventory = new Inventory();
        inventory.Add(new Consumable("potion-1", "Healing Potion"));
        inventory.Add(new Weapon("sword-1", "Iron Sword"));
        var idsBefore = inventory.All().Select(item => item.Id).ToArray();

        ItemSearch.Find(inventory, "potion");

        Assert.Equal(idsBefore, inventory.All().Select(item => item.Id));
    }
}