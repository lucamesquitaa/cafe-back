using Mono.TextTemplating;
using SaudeIA.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace SaudeIA.Models
{
  public class DetalhesModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Rede { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty;
    public int Category { get; set; }
    public bool? Child { get; set; }
    public bool? Pets { get; set; }
    public double? PetsTax { get; set; }
    public string Cep { get; set; } = String.Empty;
    public string Address { get; set; } = String.Empty;
    public string Number { get; set; } = String.Empty;
    public string? Complement { get; set; } = String.Empty;
    public string? Lobby { get; set; }
    public string? Diff { get; set; }
    public bool? Beach { get; set; }
    public bool? Downtown { get; set; }
    public bool? Airpot { get; set; }
    public bool? Highway { get; set; }
    public bool? Hospital { get; set; }
    public bool? Coffee { get; set; }
    public bool? Wifi { get; set; }
    public bool? Swimming { get; set; }
    public bool? Cleaning { get; set; }
    public bool? Gym { get; set; }
    public IEnumerable<ContatosModel> Contacts { get; set; } = new List<ContatosModel>();
    public IEnumerable<FotosDetalhesModel> Photos { get; set; } = new List<FotosDetalhesModel>();
    public IEnumerable<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();

    //esconder infos pessoais
    public string Cnpj { get; set; } = String.Empty;
    public string Razao { get; set; } = String.Empty;
    public string NomeRep { get; set; } = String.Empty;
    public string TelRep { get; set; } = String.Empty;
    public string CpfRep { get; set; } = String.Empty;
    public string EmailRep { get; set; } = String.Empty;
  }
}
