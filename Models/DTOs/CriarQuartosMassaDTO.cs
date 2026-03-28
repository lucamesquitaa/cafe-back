namespace Turify.Models.DTOs
{
  public class CriarQuartosMassaDTO
  {
    public Guid TipoQuartoId { get; set; }
    public string ModoGeracao { get; set; } = "range"; // "range" | "lista"
    public int? RangeInicio { get; set; }
    public int? RangeFim { get; set; }
    public List<int>? ListaManual { get; set; } = new List<int>();
    public string? Andar { get; set; } // opcional, aplicado a todos
  }
}
