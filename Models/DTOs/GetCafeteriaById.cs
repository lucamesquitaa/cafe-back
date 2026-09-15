namespace Cafeteria.Models.DTOs
{
  public class GetCafeteriaById
  {
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Rede { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Diferencial { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string? FotoPrincipal { get; set; }
    public TypeCafeEnum CategoriaPrincipal { get; set; }
    public DateTime CriadoEm { get; set; }
  }
}
