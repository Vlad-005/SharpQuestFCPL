namespace Capstone.Core.Models;

public sealed class Consumable(string id, string displayName, int requiredLevel = 0)
    : Item(id, displayName, ItemCategory.Consumable, requiredLevel)
{
    public override string Describe() => $"Consumable: {DisplayName}, requires level {RequiredLevel}.";
}