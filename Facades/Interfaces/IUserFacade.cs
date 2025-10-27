
using Microsoft.AspNetCore.Mvc;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace SaudeIA.Facades.Interfaces
{
  public interface IUserFacade : IRetorno
  {
    public Task<IRetorno<UserModel>> LoginAndRegisterGoogle(UserGoogleDTO userGoogle);
    public Task<IRetorno<IEnumerable<GetAllManagers>>> GetAllPermissionUsers(string hotelId);
    public Task<IRetorno> UpdatePermissionUsers(string hotelId, string emails);

  }
}
