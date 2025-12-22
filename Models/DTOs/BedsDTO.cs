using Turify.Models.Enums;

namespace Turify.Models.DTOs
{
  public class BedsDTO
  {
    public int Id{ get; set; }
    // Tipo de cama (nome do enum): "Solteiro", "Beliche", "Casal", "Queen", "King", "Berco"
    public int BedType { get; set; } 

    // Quantidade por quarto
    public int Quantity { get; set; }

  }
}
