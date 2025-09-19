namespace SaudeIA.Models.DTOs
{
  public class CadastroModelDTO
  {
    public string Name { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string EmailConfirmation { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public string? Tel { get; set; }
  }
}
