namespace Capstone.Core.Models;

public class Character
{
    public Character(string id, string displayName, int level)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        if (level < 1)
            throw new ArgumentOutOfRangeException(nameof(level));

        Id = id;
        DisplayName = displayName;
        Level = level;
        Inventory = new Inventory();
    }

    public string Id { get; }
    public string DisplayName { get; }

    public int Level { get; }

    public Inventory Inventory { get; }
}