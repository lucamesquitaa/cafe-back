using Turify.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class DetalhesModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
    public string Rede { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Se houver enum, considere usá‑lo aqui
    public int Category { get; set; }

    public bool? Child { get; set; }
    public bool? Pets { get; set; }
    public double? PetsTax { get; set; }

    public string Cep { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
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

    // Coleções navegacionais - usar ICollection para EF
    public ICollection<QuartosModel> Quartos { get; set; } = new List<QuartosModel>();
    public ICollection<ContatosModel> Contacts { get; set; } = new List<ContatosModel>();
    public ICollection<FotosDetalhesModel> Photos { get; set; } = new List<FotosDetalhesModel>();
    public ICollection<UsuarioPermissoes> Permissions { get; set; } = new List<UsuarioPermissoes>();

    public DateTime? DeletedAt { get; set; }

    // esconder infos pessoais
    public string Cnpj { get; set; } = string.Empty;
    public string Razao { get; set; } = string.Empty;
    public string NomeRep { get; set; } = string.Empty;
    public string TelRep { get; set; } = string.Empty;
    public string CpfRep { get; set; } = string.Empty;
    public string EmailRep { get; set; } = string.Empty;
  }
}
