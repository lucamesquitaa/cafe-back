using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using SaudeIA.Data;
using SaudeIA.Facades;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using SaudeIA.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaudeIA.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly Context _context;
    private readonly UserFacade _userFacade;

    public UserController(IConfiguration configuration, Context context, UserFacade userFacade)
    {
      _configuration = configuration;
      _context = context;
      _userFacade = userFacade;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserGoogleDTO userGoogle)
    {
      var result = await _userFacade.LoginAndRegisterGoogle(userGoogle);

      if (result is OkObjectResult okResult && okResult.Value is UserModel user && !string.IsNullOrEmpty(user.Email))
      {
        var token = GenerateJwtToken(user.Email, RoleUserModel.User);
        return Ok(new { token = token });
      }

      return Unauthorized("Token de login expirado.");
    }

    //[Authorize(Roles = RoleUserModel.Turify)]
    //[HttpPost("UpdateAdmin")]
    //public async Task<IActionResult> UpdateAdmin([FromBody] string email)
    //{
    //  return await _userFacade.UpdateRoleUser(email);
    //}

    [AllowAnonymous]
    [HttpPost("GetAllPermissionUsers")]
    public async Task<IEnumerable<GetAllManagers>> GetAllPermissionUsers(hotelIdObj obj)
    {
      return await _userFacade.GetAllPermissionUsers(obj.HotelId);
    }

    [AllowAnonymous]
    [HttpPost("UpdateManager")]
    public async Task<IActionResult> UpdateManager(ObjSetManager obj)
    {
      return await _userFacade.UpdatePermissionUsers(obj.HotelId, obj.Email);
    }

    private string GenerateJwtToken(string email, string? role)
    {
      role = !string.IsNullOrEmpty(role) ? role : RoleUserModel.User;
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, role)
    };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue("JwtToken", "")));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          claims: claims,
          expires: DateTime.Now.AddMinutes(30),
          signingCredentials: creds);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
