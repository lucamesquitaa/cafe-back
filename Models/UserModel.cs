using SaudeIA.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaudeIA.Models
{
  public class UserModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? GoogleId { get; set; }
    public string FirstName { get; set; } = String.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = String.Empty;
    public string? Photo { get; set; }
    public IEnumerable<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();
    public DateTime Created { get; set; } = DateTime.UtcNow;
  }
}
