namespace Turify.Models.DTOs
{
  public class UpdateDayDisponibilidadeDTO
  {
    public string Day { get; set; } = string.Empty; //yyyy-MM-dd
    public double DayPrice { get; set; }
    public int MinDays { get; set; }
    public int MaxDays { get; set; }
  }
}
