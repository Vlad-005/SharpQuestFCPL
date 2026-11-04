namespace Capstone.Core.Models;

public sealed class Armor(string id, string displayName, int requiredLevel = 0)
    : Item(id, displayName, ItemCategory.Armor, requiredLevel)
{
    public override string Describe() => $"Armor: {DisplayName}, requires level {RequiredLevel}.";
}