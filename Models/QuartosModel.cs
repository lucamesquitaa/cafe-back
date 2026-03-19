using Turify.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class QuartosModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DetalhesModelId { get; set; }

    [ForeignKey(nameof(DetalhesModelId))]
    public DetalhesModel? Detalhes { get; set; }
    public int Numero { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CategoryQuarto> Category { get; set; } = new List<CategoryQuarto>();
    public string Description { get; set; } = string.Empty;
    public int MaxOcupation { get; set; }
    public bool? Refund { get; set; }
    public string AreaSize { get; set; } = string.Empty;
    public ICollection<BedsDTO> Beds { get; set; } = new List<BedsDTO>();
    public string? Diff { get; set; }
    public bool? Freeze { get; set; }
    public bool? Vault { get; set; }
    public bool? Telephone { get; set; }
    public bool? Coffee { get; set; }
    public bool? Wifi { get; set; }
    public bool? Fridge { get; set; }
    public bool? Cleaning { get; set; }
    public bool? Varanda { get; set; }
    public bool? Bathroom { get; set; }
    public string? BathProducts { get; set; }
    public bool? Tv { get; set; }
    public string? TypeTv { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<FotosDetalhesModel> Photos { get; set; } = new List<FotosDetalhesModel>();
    public ICollection<QuartoReservas> Reservas { get; set; } = new List<QuartoReservas>();
    public ICollection<QuartoAvailable> Disponibilidade { get; set; } = new List<QuartoAvailable>();
  }
}