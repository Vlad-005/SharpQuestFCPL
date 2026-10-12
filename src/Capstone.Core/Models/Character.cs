using SharpQuest.Contracts;

namespace Capstone.Core.Models;


public class Character : IItem
{
    public Character(string id, string displayName, int lvl)
    {
        Id = id;
        DisplayName = displayName;
        Level = lvl;
    }
    public string Id { get; }
    public string DisplayName { get; }
    
    private int Level { get; }
    
}