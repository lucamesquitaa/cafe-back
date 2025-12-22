
namespace Turify.Models.DTOs
{
  public class BedTypeCreateDto
  {
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
  }

  public class BedTypeUpdateDto
  {
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
  }

  public class BedTypeResponseDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DetalhesModelId { get; set; }
  }
}