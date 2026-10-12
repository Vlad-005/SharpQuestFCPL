using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public class Item : IItem
{
    public Item(string id, string displayName, string type, int reqLvl)
    {
        Id = id;
        DisplayName = displayName;
        Type = type;
        RequiredLevel = reqLvl;
    }
    public string Id { get; }
    public string DisplayName { get; }
    
    public string Type { get; }
    
    public int RequiredLevel { get; }
}