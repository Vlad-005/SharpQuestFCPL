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

        var id = $"sample-{seed}";
        var displayName = $"Sample Item {seed}";
        var requiredLevel = (seed - 1) % 10;

        return ((seed - 1) % 3) switch
        {
            0 => new Weapon(id, displayName, requiredLevel),
            1 => new Armor(id, displayName, requiredLevel),
            _ => new Consumable(id, displayName, requiredLevel)
        };
    }
}