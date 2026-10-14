using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public sealed class InventoryStack
{
    public InventoryStack(IItem item, int quantity)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        Item = item;
        Quantity = quantity;
    }

    public IItem Item { get; }

    public int Quantity { get; }
}