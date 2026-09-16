using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafeteria.Models
{
  public class FotosDetalhesModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CafeteriaId { get; set; }

    [ForeignKey(nameof(CafeteriaId))]
    public CafeteriaModel? Cafeteria { get; set; }

    public string Alt { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool? Stared { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
  }
}
