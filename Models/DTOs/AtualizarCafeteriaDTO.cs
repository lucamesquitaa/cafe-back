namespace Turify.Models.DTOs
{
  public class AtualizarCafeteriaDTO
  {
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string? CategoriaPrincipal { get; set; }
    public string? FotoUrl { get; set; }
  }
}
