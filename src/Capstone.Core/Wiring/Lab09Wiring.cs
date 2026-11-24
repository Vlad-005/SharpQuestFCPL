using Capstone.Core.Models;
using Capstone.Core.ViewModels;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab09Wiring : ILab09Wiring
{
    private readonly Lab02Wiring _lab02 = new();

    public IItemStore CreateStore() => _lab02.CreateStore();

    public IItem CreateSample(int seed) => _lab02.CreateSample(seed);

    public ICatalogueViewModel CreateViewModel(IItemStore store)
        => new CatalogueViewModel(store);
}
