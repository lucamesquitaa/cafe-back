using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class UsuarioPermissoes
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // FK para UserModel
    public Guid UserModelId { get; set; }

    [ForeignKey(nameof(UserModelId))]
    public UserModel? User { get; set; }

    public string UserModelEmail { get; set; } = string.Empty;

    // FK para DetalhesModel
    [Column("DetalhesModelId")]
    public Guid HotelId { get; set; }

    [ForeignKey(nameof(HotelId))]
    public DetalhesModel? Detalhes { get; set; }

    // Papel/role do usuário para o detalhe
    public string Role { get; set; } = string.Empty;
  }
}
