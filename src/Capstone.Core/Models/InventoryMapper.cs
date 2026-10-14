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
            Category = gameItem?.Category ?? ItemCategory.Miscellaneous,
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

            return new Item(
                dto.Id,
                dto.DisplayName,
                dto.Category ?? ItemCategory.Miscellaneous,
                dto.RequiredLevel ?? 0);
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