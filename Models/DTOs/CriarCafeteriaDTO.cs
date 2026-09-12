namespace Turify.Models.DTOs
{
  public class CriarCafeteriaDTO
  {
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string? CategoriaPrincipal { get; set; }
    public string? FotoUrl { get; set; }
  }
}
