using System.ComponentModel;

namespace SaudeIA.Models.Enums
{
  public enum CategoryHotelModel
  {
    [Description("Hotel")]
    Day = 1,
    [Description("Toda semana")]
    Week = 2,
    [Description("Não")]
    None = 3,
  }
  public enum EstadoMetaModel
  {
    [Description("Concluído")]
    Concluido = 1,
    [Description("Em andamento")]
    EmAndamento = 2,
  }
  public enum HorarioMetaModel
  {
    [Description("Manhã")]
    Manha = 1,
    [Description("Tarde")]
    Tarde = 2,
    [Description("Noite")]
    Noite = 3,
    [Description("Dia inteiro")]
    DiaInteiro = 4,
  }
  public class RoleUserModel
  {
    public const string User = "User";
    public const string Manager = "Manager";
    public const string Admin = "Admin";
    public const string Turify = "Turify";
  }
}
