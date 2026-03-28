namespace Turify.Models.DTOs
{
  public class GetDetalheById
  {
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Rede { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Category { get; set; }
    public bool? Child { get; set; }
    public bool? Pets { get; set; }
    public double? PetsTax { get; set; }
    public string Cep { get; set; } = String.Empty;
    public string Address { get; set; } = String.Empty;
    public string Number { get; set; } = String.Empty;
    public string? Complement { get; set; } = String.Empty;
    public string? Lobby { get; set; }
    public string? Diff { get; set; }
    public bool? Beach { get; set; }
    public bool? Downtown { get; set; }
    public bool? Airpot { get; set; }
    public bool? Highway { get; set; }
    public bool? Hospital { get; set; }
    public bool? Coffee { get; set; }
    public bool? Wifi { get; set; }
    public bool? Swimming { get; set; }
    public bool? Cleaning { get; set; }
    public bool? Gym { get; set; }
    public string? DeletedAt { get; set; }
  }
}
