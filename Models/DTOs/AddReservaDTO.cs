namespace Turify.Models.DTOs
{
  public class AddReservaDTO
  {
    public int ReservaStatus { get; set; } 
    public DateTime Checkin { get; set; }
    public DateTime Checkout { get; set; }
    public bool? EarlyCheckin { get; set; }
    public bool? LateCheckout { get; set; }
    public int Adults { get; set; }
    public int Kids { get; set; }
    public string? Cupom { get; set; }
    public double PriceTotal { get; set; }
  }
}
