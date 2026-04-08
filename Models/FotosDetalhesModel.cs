using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class FotosDetalhesModel
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
  
    [Column("HotelId")]
    public Guid? HotelId { get; set; }
    [ForeignKey(nameof(HotelId))]
    public DetalhesModel? Detalhes { get; set; }

    [Column("QuartosModelId")]
    public Guid? RoomId { get; set; }
    [ForeignKey(nameof(RoomId))]
    public QuartosModel? Quartos { get; set; }

    public string Alt { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool? Stared { get; set; }
  }
}
