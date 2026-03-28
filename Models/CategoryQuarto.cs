using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Turify.Models.DTOs;

namespace Turify.Models
{
  public class CategoryQuarto
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("DetalhesModelId")]
    public Guid HotelId { get; set; }
    [ForeignKey(nameof(HotelId))]
    public DetalhesModel? Detalhes { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public int? Number { get; set; }
    public int? MinHospedes { get; set; }
    public int? MaxHospedes { get; set; }
    public ICollection<BedsDTO> ConfiguracaoCamas { get; set; } = new List<BedsDTO>();
    public bool AceitaCamaExtra { get; set; } = false;
    public bool AceitaBerco { get; set; } = false;
    public string? Descricao { get; set; }
    public DateTime? DeletedAt { get; set; }
  }
}
