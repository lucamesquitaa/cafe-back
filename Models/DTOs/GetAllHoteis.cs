
using Microsoft.Graph;

namespace Turify.Models.DTOs
{
  public class GetAllHoteis
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Url { get; set; } = String.Empty;
    public IEnumerable<string> PhotosStared { get; set; } = new List<string>();
  }
}
