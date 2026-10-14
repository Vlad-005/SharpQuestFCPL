using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public class Item : IItem
{
    public Item(
        string id,
        string displayName,
        ItemCategory category = ItemCategory.Miscellaneous,
        int requiredLevel = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        if (!Enum.IsDefined(category))
            throw new ArgumentOutOfRangeException(nameof(category));
        if (requiredLevel < 0)
            throw new ArgumentOutOfRangeException(nameof(requiredLevel));

        Id = id;
        DisplayName = displayName;
        Category = category;
        RequiredLevel = requiredLevel;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public ItemCategory Category { get; }
    public int RequiredLevel { get; }
}