using Cafeteria.Models.Enums;

namespace Cafeteria.Models.DTOs
{
  public class MensagemCafeteriaDeadLetter
  {
    public string messageId { get; set; } = string.Empty;
    public string cafeteriaId { get; set; } = string.Empty;
    public TipoRequisicao reqType { get; set; }
    public string Erro { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public DateTime FalhaEm { get; set; }
  }
}
