using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public abstract class Item : IItem
{
    protected Item(string id, string displayName, ItemCategory category, int requiredLevel = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        if (!Enum.IsDefined(category))
            throw new ArgumentOutOfRangeException(nameof(category));
        ArgumentOutOfRangeException.ThrowIfNegative(requiredLevel);

        Id = id;
        DisplayName = displayName;
        Category = category;
        RequiredLevel = requiredLevel;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public ItemCategory Category { get; }
    public int RequiredLevel { get; }

    public abstract string Describe();
}