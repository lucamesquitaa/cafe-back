namespace Turify.Models.DTOs
{
  public class AddDisponibilidadeDTO
  {
    public int Status { get; set; } //StatusReservaEnum
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DayPrice { get; set; }
    public int MinDays { get; set; }
    public int MaxDays { get; set; }
    public bool Reembolsavel { get; set; } = true;
  }
}
