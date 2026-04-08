using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Facades.Interfaces
{
  public interface IQuartosFacade : IRetorno
  {
    public Task<IRetorno<IEnumerable<QuartosModel>>> GetAllQuartos(string HotelId);
    public Task<IRetorno<QuartosModel>> GetQuartoById(string quartoId);
    public Task<IRetorno<QuartosModel>> PostQuartosFacade(QuartosModel quartos, string HotelId);
    public Task<IRetorno<QuartosMassaResultDTO>> PostQuartosMassa(string HotelId, CriarQuartosMassaDTO dto);
  }
}
