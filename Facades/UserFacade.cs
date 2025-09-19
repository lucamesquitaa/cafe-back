using BCrypt.Net;
using Google.Apis.Auth;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using SaudeIA.Data;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using SaudeIA.Models.Enums;
using SaudeIA.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace SaudeIA.Facades
{
  public class UserFacade : IUserFacade
  {
    private readonly Context _context;
    private readonly GoogleAuthService _googleAuthService;
    
    public UserFacade(Context context, GoogleAuthService googleAuthService)
    {
      _context = context;
      _googleAuthService = googleAuthService;
    }

    public async Task<IActionResult> LoginAndRegisterGoogle(UserGoogleDTO userGoogle)
    {
      try
      {

        if (userGoogle == null)
          return new UnauthorizedObjectResult("Token inválido.");

        var googleId = userGoogle.Id;
        var email = userGoogle.Email;
        var firstName = userGoogle.FirstName;
        var lastName = userGoogle.LastName;
        var picture = userGoogle.Photo;

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
          user = new UserModel
          {
            Id = Guid.NewGuid(),
            GoogleId = googleId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Photo = picture
          };
          _context.Usuarios.Add(user);
          await _context.SaveChangesAsync();
        }

        return new OkObjectResult(user);
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public async Task<IEnumerable<GetAllManagers>> GetAllPermissionUsers(string hotelId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return null;

        // Fix: Use FirstOrDefaultAsync to retrieve a single value instead of IQueryable
        var userId = await _context.Usuarios
            .Where(u => u.Email == userEmail)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (userId == Guid.Empty)
          return null;

        var hotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Id.ToString() == hotelId);
        if (hotel == null)
        {
          return null;
        }

        var userIsAdmin = await _context.UsuarioPermissao
            .FirstOrDefaultAsync(x => x.UserModelId == userId && x.DetalhesModelId == Guid.Parse(hotelId) &&
                                      (x.Role == RoleUserModel.Manager || x.Role == RoleUserModel.Admin || x.Role == RoleUserModel.Turify));

        if (userIsAdmin == null)
          return null;

        var users = await _context.UsuarioPermissao.Where(u => u.DetalhesModelId == Guid.Parse(hotelId)).Select(x => new GetAllManagers { Email = x.UserModelEmail, Role = x.Role}).ToListAsync();


        if (users == null)
          return null;
        
        return users;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public async Task<IActionResult> UpdatePermissionUsers(string hotelId, string email)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return new UnauthorizedObjectResult("Token inválido.");

        // Fix: Use FirstOrDefaultAsync to retrieve a single value instead of IQueryable
        var userId = await _context.Usuarios
            .Where(u => u.Email == userEmail)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (userId == Guid.Empty)
          return new UnauthorizedObjectResult("Usuário não encontrado.");

        var hotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Id.ToString() == hotelId);
        if (hotel == null)
        {
          return new BadRequestObjectResult("Hotel não encontrado.");
        }

        var userIsAdmin = await _context.UsuarioPermissao
            .FirstOrDefaultAsync(x => x.UserModelId == userId && x.DetalhesModelId == Guid.Parse(hotelId) &&
                                      (x.Role == RoleUserModel.Admin || x.Role == RoleUserModel.Turify));

        if (userIsAdmin == null)
          return new BadRequestObjectResult("Permissão do usuário não é admin deste hotel.");

        var newRole = "";

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
          return new NotFoundObjectResult($"Usuário {email} não encontrado.");

        var isManagerAlready = await _context.UsuarioPermissao
            .FirstOrDefaultAsync(x => x.UserModelId == user.Id && x.DetalhesModelId == Guid.Parse(hotelId));

        if (isManagerAlready == null)
        {
          newRole = RoleUserModel.Manager;

          var permissions = new UsuarioPermissoes
          {
            Id = Guid.NewGuid(),
            DetalhesModelId = Guid.Parse(hotelId),
            UserModelEmail = email,
            UserModelId = user.Id,
            Role = newRole
          };
          await _context.UsuarioPermissao.AddAsync(permissions);
        }
        else
        {
          _context.UsuarioPermissao.Remove(isManagerAlready);
        }

        await _context.SaveChangesAsync();

        return new OkResult();
      }
      catch (Exception e)
      {
        return null;
      }
    }
  }
}
