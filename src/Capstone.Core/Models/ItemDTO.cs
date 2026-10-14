namespace Capstone.Core.Models;

public record ItemDTO
{
    public string? Id { get; set; }

    public string? DisplayName { get; set; }

    public ItemCategory? Category { get; set; }

    public int? RequiredLevel { get; set; }
}