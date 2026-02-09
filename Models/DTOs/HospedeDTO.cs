using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models.DTOs
{
  public class HospedeDTO
  {
    public string? ReservationId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string FamilyName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string CPF { get; set; } = String.Empty;
    public string Phone { get; set; } = String.Empty;
    public string DateBirth { get; set; } = String.Empty;
    public string CEP { get; set; } = String.Empty;
    public string State { get; set; } = String.Empty;
    public string City { get; set; } = String.Empty;
    public string Address { get; set; } = String.Empty;
    public string? Complement { get; set; }
    public string? BloodType { get; set; }
    public bool? Principal { get; set; }
    public string? TextArea { get; set; }
  }
}
