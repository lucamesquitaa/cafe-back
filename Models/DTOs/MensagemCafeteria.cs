using Cafeteria.Models.Enums;

namespace Cafeteria.Models.DTOs
{
  public class MensagemCafeteria
  {
    public string cafeteriaId { get; set; } = string.Empty;
    public TipoRequisicao reqType { get; set; }
  }
}
