namespace Turify.Models.DTOs
{
  public class GetAllCafeterias
  {
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string? FotoPrincipal { get; set; }
    public TypeCafeEnum CategoriaPrincipal { get; set; }
  }
}
