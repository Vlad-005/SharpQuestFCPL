using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public class Lab02Wiring : ILab02Wiring
{
    public IItemStore CreateStore() => new Inventory();

    public IItemMapper CreateMapper() => new InventoryMapper();

    public IItem CreateSample(int seed)
    {
        if (seed is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(seed), "Seed must be between 1 and 100.");

        var categories = Enum.GetValues<ItemCategory>();
        return new Item(
            $"sample-{seed}",
            $"Sample Item {seed}",
            categories[(seed - 1) % categories.Length],
            (seed - 1) % 10);
    }
}