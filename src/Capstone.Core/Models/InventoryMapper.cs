using System.Text.Json;
using SharpQuest.Contracts;

namespace Capstone.Core.Models;

public class InventoryMapper : IItemMapper
{
    public string ToJson(IItem item)
    {
        throw new NotImplementedException();
    }

    public IItem? FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<ItemDTO>(json);
        dynamic? x;
        x = dto ?? new ItemDTO();
        x = 4;

        if (dto == null || string.IsNullOrEmpty(dto.Id) || string.IsNullOrWhiteSpace(dto.DisplayName))
            return null;
    
        Convert.ToInt32(dto.RequiredLevel);

        return new Item(dto.Id, dto.DisplayName, dto.Type ?? string.Empty, int.Parse(dto.RequiredLevel ?? "1"));


    }
}