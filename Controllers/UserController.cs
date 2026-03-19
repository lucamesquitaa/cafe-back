using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using Turify.Data;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Turify.Controllers
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

      if (result != null && result.Data != null && !string.IsNullOrEmpty(result.Data.Email))
      {
        var token = GenerateJwtToken(result.Data.Email, RoleUserModel.User);
        return Ok(new { token = token });
      }

      return Unauthorized("Token de login expirado.");
    }

    [HttpPost("GetAllPermissionUsers")]
    public async Task<IActionResult> GetAllPermissionUsers([FromBody] hotelIdObj obj)
    {
       var result = await _userFacade.GetAllPermissionUsers(obj.HotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [HttpPost("UpdateManager")]
    public async Task<IActionResult> UpdateManager([FromBody] ObjSetManager obj)
    {
      var result = await _userFacade.UpdatePermissionUsers(obj.HotelId, obj.Email);

      if(result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [HttpDelete("RemovePermissionUsers")]
    public async Task<IActionResult> RemovePermissionUsers([FromBody] ObjSetManager obj)
    {
      var result = await _userFacade.RemovePermissionUsers(obj.HotelId, obj.Email);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
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
