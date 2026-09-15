using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Models.DTOs
{
  public class UserGoogleDTO
  {
    [Required]
    public string IdToken { get; set; } = string.Empty;
  }
}
