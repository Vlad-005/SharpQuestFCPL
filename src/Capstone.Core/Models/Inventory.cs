using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public class Inventory : IItemStore
{
    public readonly List<Item> Items = new List<Item>();
    
    public int Count { get;  }
    
    public void Add(Item item)
    {
        throw new NotImplementedException();
    }

    public void Add(IItem item)
    {
        throw new NotImplementedException();
    }

    public IItem? FindById(string id)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<IItem> All()
    {
        throw new NotImplementedException();
    }

    public bool Remove(string id)
    {
        throw new NotImplementedException();
    }
    
}