
using Cafeteria.Models;
using Cafeteria.Models.DTOs;

namespace Cafeteria.Facades.Interfaces
{
  public interface IUserFacade : IRetorno
  {
    public Task<IRetorno<UserModel>> LoginAndRegisterGoogle(UserGoogleDTO userGoogle);
    public Task<IRetorno<IEnumerable<GetAllManagers>>> GetAllPermissionUsers(string CafeteriaId);
    public Task<IRetorno> UpdatePermissionUsers(string CafeteriaId, string email);
    public Task<IRetorno> RemovePermissionUsers(string CafeteriaId, string email);
  }
}
