using System.ComponentModel.DataAnnotations;

namespace Turify.Models
{
  public class Cafeteria
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Endereco { get; set; } = string.Empty;

    public double Lat { get; set; }
    public double Lng { get; set; }

    public double NotaMedia { get; set; } = 0;
    public int QtdAvaliacoes { get; set; } = 0;

    public string? FotoUrl { get; set; }
    public string? CategoriaPrincipal { get; set; }

    public ICollection<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
  }
}
