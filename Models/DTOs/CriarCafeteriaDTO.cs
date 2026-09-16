using Microsoft.AspNetCore.Http;

namespace Cafeteria.Models.DTOs
{
  public class CriarCafeteriaDTO
  {
    public string Nome { get; set; } = string.Empty;
    public string Rede { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Diferencial { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public IFormFile? FotoPrincipal { get; set; }

    //infos privadas

    public string Cnpj { get; set; } = string.Empty;
    public string Razao { get; set; } = string.Empty;
    public string NomeRep { get; set; } = string.Empty;
    public string TelRep { get; set; } = string.Empty;
    public string CpfRep { get; set; } = string.Empty;
    public string EmailRep { get; set; } = string.Empty;
  }
}
