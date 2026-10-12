using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public class Lab02Wiring
{
    public IItemStore CreateStore() => new Inventory();
    public IItemMapper CreateMapper() => new InventoryMapper();

    
    
}