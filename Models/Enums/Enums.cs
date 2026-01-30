using System.ComponentModel;

namespace Turify.Models.Enums
{
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

  public enum StatusReservaEnum
  {
    [Description("Disponível")]
    Disponivel = 1,
    [Description("Pré-Reserva")]
    PreReserva = 2,
    [Description("Aguardando pagamento")]
    AguardandoPagamento = 3,
    [Description("Bloqueado")]
    Bloqueado = 4,
    [Description("Confirmada")]
    Confirmada = 5,
  }

  public enum StatusHospedeReservaEnum
  {
    [Description("Pagamento pendente")]
    PagamentoPendente = 1,
    [Description("Pagamento confirmado")]
    PagamentoConfirmado = 2,
    [Description("Check-in realizado")]
    CheckInRealizado = 3,
    [Description("Check-out realizado")]
    CheckOutRealizado = 4,
    [Description("Cancelado pelo hotel")]
    CanceladaHotel = 5,
    [Description("Cancelado pelo usuário")]
    CanceladaUsuario = 6,
    [Description("Não comparecimento")]
    NaoComparecimento = 7,
  }

  public class RoleUserModel
  {
    public const string User = "User";
    public const string Manager = "Manager";
    public const string Admin = "Admin";
    public const string Turify = "Turify";
  }
}
