using Cafeteria.Models.Enums;

namespace Cafeteria.Models.DTOs
{
  public class MensagemCafeteria
  {
    //idempotencia
    public string messageId { get; set; } = string.Empty;
    public string cafeteriaId { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public TipoRequisicao reqType { get; set; }
    public string photo { get; set; }
    public DateTime date { get; set; } 
  }
}
