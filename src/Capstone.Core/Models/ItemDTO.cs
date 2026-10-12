namespace Capstone.Core.Models;

public record ItemDTO
{
    public string? Id { get; set; }
    
    public string? DisplayName { get; set; }
    
    public string? Type { get; set; }
    
    public string? RequiredLevel { get; set; }
}