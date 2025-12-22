using Microsoft.AspNetCore.Mvc;
using Turify.Models.DTOs;
using Turify.Models;

namespace Turify.Facades.Interfaces
{
  public interface IHotelFacade : IRetorno
  {
    public Task<IRetorno<IEnumerable<GetAllHoteis>>> GetAllFacade();
    public Task<IRetorno<GetDetalheById>> GetDetalhesFacade(string hotelId);
    public Task<IRetorno<DetalhesModel>> GetDetalhesFacadeByManager(string hotelId);
    public Task<IRetorno<IEnumerable<GetAllHoteis>>> GetDetalhesUserFacade();
    public Task<IRetorno> PostDetalhesFacade(DetalhesModel hotel);
    public Task<IRetorno> PutDetalhesFacade(DetalhesModel hotel, string hotelId);
    public Task<IRetorno> DeleteDetalhesFacade(string id);
  }
}
