namespace Turify.Models.DTOs
{
  public class GetAllCafeterias
  {
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string? CategoriaPrincipal { get; set; }
    public double NotaMedia { get; set; }
    public int QtdAvaliacoes { get; set; }
    public double? DistanciaKm { get; set; }
  }
}
