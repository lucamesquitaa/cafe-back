namespace Turify.Models.DTOs
{
  public class GetCafeteriaById
  {
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public double NotaMedia { get; set; }
    public int QtdAvaliacoes { get; set; }
    public string? FotoUrl { get; set; }
    public string? CategoriaPrincipal { get; set; }
    public DateTime CriadoEm { get; set; }
  }
}
