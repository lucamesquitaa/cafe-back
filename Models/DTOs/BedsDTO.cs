using System.ComponentModel.DataAnnotations.Schema;

namespace SaudeIA.Models.DTOs
{
  public class BedsDTO
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
  }
}
