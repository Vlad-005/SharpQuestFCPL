using System.Text.Json;
using System.Text.Json.Serialization;
using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab08Wiring : ILab08Wiring
{
    private readonly Lab02Wiring _lab02 = new();
    private readonly Lab04Wiring _lab04 = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    public IItemStore CreateStore() => _lab02.CreateStore();

    public IItem CreateSample(int seed) => _lab02.CreateSample(seed);

    public IReadOnlyList<Type> KindTypes => _lab04.KindTypes;

    public IItem CreateOfKind(int kindIndex, int seed) => _lab04.CreateOfKind(kindIndex, seed);

    public async Task SaveAsync(IItemStore store, string path)
    {
        ArgumentNullException.ThrowIfNull(store);
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be blank.", nameof(path));

        var inventory = GetInventory(store);
        var document = new InventorySaveDocument(
            inventory.Stacks.Select(stack => new InventoryItemSaveDto
            {
                Id = stack.Item.Id,
                DisplayName = stack.Item.DisplayName,
                Kind = GetKind(stack.Item),
                RequiredLevel = stack.Item is Item item ? item.RequiredLevel : null,
                Quantity = stack.Quantity
            }).ToArray());

        var folder = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(folder))
            Directory.CreateDirectory(folder);

        await using var stream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);
        await JsonSerializer.SerializeAsync(stream, document, JsonOptions);
        await stream.FlushAsync();
    }

    public async Task<IItemStore> LoadAsync(string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be blank.", nameof(path));

        if (!File.Exists(path))
            return new Inventory();

        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var document = await JsonSerializer.DeserializeAsync<InventorySaveDocument>(
                stream,
                JsonOptions);

            if (document?.Items is null)
                throw new InvalidDataException("The inventory file does not contain valid inventory data.");

            var inventory = new Inventory();
            var ids = new HashSet<string>(StringComparer.Ordinal);

            foreach (var row in document.Items)
            {
                if (row is null ||
                    string.IsNullOrWhiteSpace(row.Id) ||
                    string.IsNullOrWhiteSpace(row.DisplayName) ||
                    row.RequiredLevel is null or < 0 ||
                    row.Quantity is null or <= 0 ||
                    !ids.Add(row.Id))
                {
                    throw new InvalidDataException("The inventory file contains invalid item data.");
                }

                Item item = row.Kind switch
                {
                    InventoryItemKind.Weapon => new Weapon(row.Id, row.DisplayName, row.RequiredLevel.Value),
                    InventoryItemKind.Armor => new Armor(row.Id, row.DisplayName, row.RequiredLevel.Value),
                    InventoryItemKind.Consumable => new Consumable(row.Id, row.DisplayName, row.RequiredLevel.Value),
                    _ => throw new InvalidDataException("The inventory file contains an unknown item kind.")
                };

                inventory.AddToStack(item, row.Quantity.Value);
            }

            return inventory;
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The inventory file contains invalid JSON.", exception);
        }
        catch (NotSupportedException exception)
        {
            throw new InvalidDataException("The inventory file has an unsupported data shape.", exception);
        }
    }

    private static Inventory GetInventory(IItemStore store)
        => store as Inventory
           ?? throw new ArgumentException("The store is not an Inventory.", nameof(store));

    private static InventoryItemKind GetKind(IItem item) => item switch
    {
        Weapon => InventoryItemKind.Weapon,
        Armor => InventoryItemKind.Armor,
        Consumable => InventoryItemKind.Consumable,
        _ => throw new ArgumentException("The inventory contains an unsupported item kind.", nameof(item))
    };

    private sealed record InventorySaveDocument(InventoryItemSaveDto[]? Items);

    private sealed class InventoryItemSaveDto
    {
        public string? Id { get; init; }
        public string? DisplayName { get; init; }
        public InventoryItemKind Kind { get; init; }
        public int? RequiredLevel { get; init; }
        public int? Quantity { get; init; }
    }

    private enum InventoryItemKind
    {
        Weapon,
        Armor,
        Consumable
    }
}
