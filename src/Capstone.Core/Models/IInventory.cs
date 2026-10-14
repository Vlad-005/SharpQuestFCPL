namespace Capstone.Core.Models;

public interface IInventory
{
    int Count { get; }

    IReadOnlyList<InventoryStack> Stacks { get; }

    void AddToStack(Item item, int quantity = 1);

    bool RemoveFromStack(string itemId, int quantity = 1);
}