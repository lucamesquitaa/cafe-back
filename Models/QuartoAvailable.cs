using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class QuartoAvailable
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? QuartosModelId { get; set; }
    [ForeignKey(nameof(QuartosModelId))]
    public QuartosModel? Quartos { get; set; }
    public bool isAvailable { get; set; }
    public int? ReservationId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = String.Empty;
    public int Status { get; set; } //StatusReservaEnum
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DayPrice { get; set; }
    public int MinDays { get; set; }
    public int MaxDays { get; set; }
    public bool Reembolsavel { get; set; } = true;
  }
}
