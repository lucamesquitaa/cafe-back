using Microsoft.AspNetCore.Mvc;
using SaudeIA.Models.DTOs;
using SaudeIA.Models;

namespace SaudeIA.Facades.Interfaces
{
  public interface IHotelFacade
  {
    public Task<IEnumerable<GetAllHoteis>> GetAllFacade();
    public Task<GetDetalheById> GetDetalhesFacade(string hotelId);
    public Task<DetalhesModel> GetDetalhesFacadeByManager(string hotelId);
    public Task<IEnumerable<GetAllHoteis>> GetDetalhesUserFacade();
    public Task<IActionResult> PostDetalhesFacade(DetalhesModel hotel);
    public Task<IActionResult> PutDetalhesFacade(DetalhesModel hotel, string hotelId);
    public Task<IActionResult> DeleteDetalhesFacade(string id);
  }
}
