using Cafeteria.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafeteria.Models
{
  public class UserModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? GoogleId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public ICollection<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();
    public DateTime Created { get; set; } = DateTime.UtcNow;
  }
}
