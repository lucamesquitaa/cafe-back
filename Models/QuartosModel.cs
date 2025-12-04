using SaudeIA.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaudeIA.Models
{
  public class QuartosModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DetalhesModelId { get; set; }

    [ForeignKey(nameof(DetalhesModelId))]
    public DetalhesModel? Detalhes { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<CategoryQuarto> Category { get; set; } = new List<CategoryQuarto>();
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Description { get; set; } = string.Empty;
    public int MaxOcupation { get; set; }
    public bool? Refund { get; set; }
    public string AreaSize { get; set; } = string.Empty;
    public IEnumerable<BedsDTO> Beds { get; set; } = new List<BedsDTO>();
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
    public ICollection<FotosDetalhesModel> Photos { get; set; } = new List<FotosDetalhesModel>();
  }
}