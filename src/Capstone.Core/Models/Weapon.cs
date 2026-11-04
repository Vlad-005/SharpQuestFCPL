namespace Capstone.Core.Models;

public sealed class Weapon(string id, string displayName, int requiredLevel = 0)
    : Item(id, displayName, ItemCategory.Weapon, requiredLevel)
{
    public override string Describe() => $"Weapon: {DisplayName}, requires level {RequiredLevel}.";
}