using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaudeIA.Models
{
  public class UsuarioPermissoes
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("UserModelId")]
    public Guid UserModelId { get; set; }
    public string UserModelEmail { get; set; } = String.Empty;
    [ForeignKey("DetalhesModelId")]
    public Guid DetalhesModelId { get; set; }
    public string Role { get; set; } = String.Empty;
  }
}
