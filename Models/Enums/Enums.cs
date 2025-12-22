using System.ComponentModel;

namespace Turify.Models.Enums
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
  public enum BedTypeEnum
  {
    [Description("Solteiro")]
    Solteiro = 1,
    [Description("Beliche")]
    Beliche = 2,
    [Description("Casal")]
    Casal = 3,
    [Description("Queen")]
    Queen = 4,
    [Description("King")]
    King = 5,
    [Description("Berco")]
    Berco = 6,
  }
  public class RoleUserModel
  {
    public const string User = "User";
    public const string Manager = "Manager";
    public const string Admin = "Admin";
    public const string Turify = "Turify";
  }
}
