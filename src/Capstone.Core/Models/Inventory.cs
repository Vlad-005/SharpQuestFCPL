using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public class Inventory : IItemStore, IInventory
{
    private readonly List<InventoryStack> _stacks = [];

    public int Count => _stacks.Count;

    public IReadOnlyList<InventoryStack> Stacks => _stacks.ToArray();

    public void Add(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_stacks.Any(stack => string.Equals(stack.Item.Id, item.Id, StringComparison.Ordinal)))
            throw new ArgumentException($"An item with Id '{item.Id}' is already in the inventory.", nameof(item));

        _stacks.Add(new InventoryStack(item, 1));
    }

    public IItem? FindById(string id)
    {
        return _stacks
            .FirstOrDefault(stack => string.Equals(stack.Item.Id, id, StringComparison.Ordinal))
            ?.Item;
    }

    public IReadOnlyList<IItem> All()
    {
        return _stacks.Select(stack => stack.Item).ToArray();
    }

    public bool Remove(string id)
    {
        var index = _stacks.FindIndex(stack => string.Equals(stack.Item.Id, id, StringComparison.Ordinal));
        if (index < 0)
            return false;

        _stacks.RemoveAt(index);
        return true;
    }

    public void AddToStack(Item item, int quantity = 1)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        var index = _stacks.FindIndex(stack => string.Equals(stack.Item.Id, item.Id, StringComparison.Ordinal));
        if (index < 0)
        {
            _stacks.Add(new InventoryStack(item, quantity));
            return;
        }

        var existing = _stacks[index];
        if (existing.Item is not Item existingItem ||
            existingItem.DisplayName != item.DisplayName ||
            existingItem.Category != item.Category ||
            existingItem.RequiredLevel != item.RequiredLevel)
        {
            throw new ArgumentException("An item with the same Id but different item data is already in the inventory.",
                nameof(item));
        }

        _stacks[index] = new InventoryStack(existing.Item, checked(existing.Quantity + quantity));
    }

    public bool RemoveFromStack(string itemId, int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        var index = _stacks.FindIndex(stack => string.Equals(stack.Item.Id, itemId, StringComparison.Ordinal));
        if (index < 0 || _stacks[index].Quantity < quantity)
            return false;

        var existing = _stacks[index];
        if (existing.Quantity == quantity)
            _stacks.RemoveAt(index);
        else
            _stacks[index] = new InventoryStack(existing.Item, existing.Quantity - quantity);

        return true;
    }
}