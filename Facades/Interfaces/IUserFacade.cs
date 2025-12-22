
using Microsoft.AspNetCore.Mvc;
using Turify.Models;
using Turify.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Turify.Facades.Interfaces
{
  public interface IUserFacade : IRetorno
  {
    public Task<IRetorno<UserModel>> LoginAndRegisterGoogle(UserGoogleDTO userGoogle);
    public Task<IRetorno<IEnumerable<GetAllManagers>>> GetAllPermissionUsers(string hotelId);
    public Task<IRetorno> UpdatePermissionUsers(string hotelId, string emails);

  }
}
