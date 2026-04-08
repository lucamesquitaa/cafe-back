using Microsoft.AspNetCore.Mvc;
using Turify.Models.DTOs;
using Turify.Models;

namespace Turify.Facades.Interfaces
{
  public interface IHotelFacade : IRetorno
  {
    public Task<IRetorno<IEnumerable<GetAllHoteis>>> GetAllFacade();
    public Task<IRetorno<GetDetalheById>> GetDetalhesFacade(string HotelId);
    public Task<IRetorno<DetalhesModel>> GetDetalhesFacadeByManager(string HotelId);
    public Task<IRetorno<IEnumerable<GetAllHoteis>>> GetDetalhesUserFacade();
    public Task<IRetorno<DetalhesModel>> PostDetalhesFacade(DetalhesModel hotel);
    public Task<IRetorno<DetalhesModel>> PutDetalhesFacade(DetalhesModel hotel, string HotelId);
    public Task<IRetorno> DeleteDetalhesFacade(string id);
  }
}
