using Microsoft.AspNetCore.Mvc;
using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Facades.Interfaces
{
  public interface IMotorDeReservasFacade : IRetorno
  {
    Task<IRetorno<QuartoAvailable>> PostDisponibilidadeAsync([FromBody] AddDisponibilidadeDTO disponibilidade, string quartoId);
    Task<IRetorno<IEnumerable<QuartoAvailable>>> GetDisponibilidadeAsync(string quartoId);
    Task<IRetorno<QuartoReservas>> PostReservaAsync([FromBody] AddReservaDTO reserva, string quartoId);
    Task<IRetorno<IEnumerable<QuartoReservas>>> GetReservaAsync(string quartoId);
    Task<IRetorno<QuartoAvailable>> PutDisponibilidadeDayAsync([FromBody] UpdateDayDisponibilidadeDTO disponibilidadeDay, string quartoId);
  }
}
