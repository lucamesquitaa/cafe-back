using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaudeIA.Models
{
  public class FotosDetalhesModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
  
    public Guid DetalhesModelId { get; set; }
    [ForeignKey(nameof(DetalhesModelId))]
    public DetalhesModel? Detalhes { get; set; }

    public Guid QuartosModelId { get; set; }
    [ForeignKey(nameof(QuartosModelId))]
    public QuartosModel? Quartos { get; set; }

    public string Alt { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool? Stared { get; set; }
  }
}
