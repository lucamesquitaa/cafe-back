
using Microsoft.AspNetCore.Mvc;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace SaudeIA.Facades.Interfaces
{
  public interface IUserFacade
  {
    public Task<IActionResult> LoginAndRegisterGoogle(UserGoogleDTO userGoogle);
    public Task<IEnumerable<GetAllManagers>> GetAllPermissionUsers(string hotelId);
    public Task<IActionResult> UpdatePermissionUsers(string hotelId, string emails);

  }
}
