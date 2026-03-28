namespace Turify.Models.DTOs
{
  public class QuartosMassaResultDTO
  {
    public int Criados { get; set; }
    public List<int> Numeros { get; set; } = new();
    public List<int> Conflitos { get; set; } = new();
  }
}
