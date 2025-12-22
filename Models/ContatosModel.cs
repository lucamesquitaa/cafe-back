using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class ContatosModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // FK explícita
    public Guid DetalhesModelId { get; set; }

    // Navegação com ForeignKey apontando para a FK acima
    [ForeignKey(nameof(DetalhesModelId))]
    public DetalhesModel? Detalhes { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Contact { get; set; } = string.Empty;
  }
}
