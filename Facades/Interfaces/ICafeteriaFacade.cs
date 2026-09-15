using Turify.Models.DTOs;

namespace Turify.Facades.Interfaces
{
  public interface ICafeteriaFacade : IRetorno
  {
    public Task<IRetorno<IEnumerable<GetAllCafeterias>>> GetAllFacade(int page, int pageSize);
    public Task<IRetorno<GetCafeteriaById>> GetByIdFacade(string id);
    public Task<IRetorno<GetCafeteriaById>> PostFacade(CriarCafeteriaDTO cafeteria);
    public Task<IRetorno<GetCafeteriaById>> PutFacade(AtualizarCafeteriaDTO cafeteria, string id);
  }
}
