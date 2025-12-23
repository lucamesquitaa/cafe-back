using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Facades.Interfaces
{
  public interface IQuartosFacade : IRetorno
  {
    public Task<IRetorno<IEnumerable<QuartosModel>>> GetAllQuartos(string hotelId);
    public Task<IRetorno<QuartosModel>> GetQuartoById(string quartoId);
    public Task<IRetorno<QuartosModel>> PostQuartosFacade(QuartosModel quartos, string hotelId);
  }
}
