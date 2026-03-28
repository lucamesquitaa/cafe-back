namespace Turify.Models.DTOs
{
  public class RetornoReservasDTO
  {
      public string? Id { get; set; }
      public string? RoomId { get; set; }
      public int ReservaStatus { get; set; }
      public DateTime Checkin { get; set; }
      public DateTime Checkout { get; set; }
      public bool? EarlyCheckin { get; set; }
      public bool? LateCheckout { get; set; }
      public int Adults { get; set; }
      public int Kids { get; set; }
      public string? Cupom { get; set; }
      public double PriceTotal { get; set; }
      public DateTime CreatedAt { get; set; }
      public ICollection<HospedeDTO> Hospede { get; set; } = new List<HospedeDTO>();
    
  }
}
