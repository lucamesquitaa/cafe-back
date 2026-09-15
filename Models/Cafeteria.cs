using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Models
{
  public class Cafeteria
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Rede { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Diferencial { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string? FotoPrincipal { get; set; }
    public TypeCafeEnum CategoriaPrincipal { get; set; }

    //infos privadas

    public string Cnpj { get; set; } = string.Empty;
    public string Razao { get; set; } = string.Empty;
    public string NomeRep { get; set; } = string.Empty;
    public string TelRep { get; set; } = string.Empty;
    public string CpfRep { get; set; } = string.Empty;
    public string EmailRep { get; set; } = string.Empty;
    
    public ICollection<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;


  }

  public enum TypeCafeEnum
  {
    Cafeteria = 1
  }
}
