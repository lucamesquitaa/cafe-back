namespace Turify.Models.DTOs
{
  public class UpdateReservaDTO
  {
    public string ReservaId { get; set; } = String.Empty;
    public int ReservaStatus { get; set; }
    public int Adults { get; set; }
    public int Kids { get; set; }
    public double PriceTotal { get; set; }
    public ICollection<HospedeDTO> Hospede { get; set; } = new List<HospedeDTO>();
  }
}
