using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class QuartoReservas
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? QuartosModelId { get; set; }
    [ForeignKey(nameof(QuartosModelId))]
    public QuartosModel? Quartos { get; set; }
    public int ReservaStatus { get; set; } //StatusHospedeReservaEnum
    public DateTime Checkin { get; set; }
    public DateTime Checkout { get; set; }
    public bool? EarlyCheckin { get; set; }
    public bool? LateCheckout { get; set; }
    public int Adults { get; set; }
    public int Kids { get; set; }
    public string? Cupom { get; set; }
    public double PriceTotal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
    public ICollection<Hospedes> Hospede { get; set; } = new List<Hospedes>();
  }
}
