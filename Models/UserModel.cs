using Turify.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class UserModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? GoogleId { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Photo { get; set; }

    // Coleção de navegação — ICollection para permitir Add/Remove e tracking do EF
    public ICollection<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();

    public DateTime Created { get; set; } = DateTime.UtcNow;
  }
}
