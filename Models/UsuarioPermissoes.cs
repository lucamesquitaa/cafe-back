using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafeteria.Models
{
  public class UsuarioPermissoes
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserModelId { get; set; }

    [ForeignKey(nameof(UserModelId))]
    public UserModel? User { get; set; }
    public string UserModelEmail { get; set; } = string.Empty;
    public Guid CafeteriaId { get; set; }

    [ForeignKey(nameof(CafeteriaId))]
    public CafeteriaModel? Cafeteria { get; set; }
    public string Role { get; set; } = string.Empty;
  }
}
