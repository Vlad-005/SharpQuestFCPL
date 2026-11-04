using System.Text.Json;
using System.Text.Json.Serialization;
using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public class InventoryMapper : IItemMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string ToJson(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var gameItem = item as Item;
        var dto = new ItemDTO
        {
            Id = item.Id,
            DisplayName = item.DisplayName,
            Category = gameItem?.Category ?? ItemCategory.Consumable,
            RequiredLevel = gameItem?.RequiredLevel ?? 0
        };
        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    public IItem? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var dto = JsonSerializer.Deserialize<ItemDTO>(json, JsonOptions);
            if (dto is null ||
                string.IsNullOrWhiteSpace(dto.Id) ||
                string.IsNullOrWhiteSpace(dto.DisplayName) ||
                dto.RequiredLevel < 0 ||
                (dto.Category is { } category && !Enum.IsDefined(category)))
            {
                return null;
            }

            var id = dto.Id;
            var displayName = dto.DisplayName;
            var requiredLevel = dto.RequiredLevel ?? 0;

            return (dto.Category ?? ItemCategory.Consumable) switch
            {
                ItemCategory.Weapon => new Weapon(id, displayName, requiredLevel),
                ItemCategory.Armor => new Armor(id, displayName, requiredLevel),
                ItemCategory.Consumable => new Consumable(id, displayName, requiredLevel),
                _ => null
            };
        }
        catch (JsonException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}