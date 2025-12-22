using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class CategoryQuarto
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DetalhesModelId { get; set; }
    [ForeignKey(nameof(DetalhesModelId))]
    public DetalhesModel? Detalhes { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public int? Number { get; set; }
  }
}
