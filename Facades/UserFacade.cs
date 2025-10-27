using Microsoft.EntityFrameworkCore;
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
  public class UserFacade : IUserFacade, IRetorno
  {
    private readonly Context _context;
    private readonly GoogleAuthService _googleAuthService;
    private UtilsFacade _utilsFacade ;

    public UserFacade(Context context, GoogleAuthService googleAuthService, UtilsFacade utilsFacade)
    {
      _context = context;
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
    }

    // Implementation of IRetorno properties  
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }

    public async Task<IRetorno<UserModel>> LoginAndRegisterGoogle(UserGoogleDTO userGoogle)
    {
      try
      {
        if (userGoogle == null)
          return Retorno<UserModel>.Erro("Token inválido.");

        var googleId = userGoogle.Id;
        var email = userGoogle.Email;
        var firstName = userGoogle.FirstName;
        var lastName = userGoogle.LastName;
        var picture = userGoogle.Photo;

        var user = await _utilsFacade.GetUserByEmail(email);

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

        return Retorno<UserModel>.Ok(user, "Login ou registro realizado com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<UserModel>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno<IEnumerable<GetAllManagers>>> GetAllPermissionUsers(string hotelId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Erro ao buscar usuário.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Erro ao buscar usuário.");

        var hotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Id.ToString() == hotelId);

        if (hotel == null)
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Erro ao buscar hotel.");

        var userIsAdmin = await _utilsFacade.IsAdminOrManager(user.Id.ToString(), hotelId);

        if (!userIsAdmin)
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Usuário não possui permissão para acessar esta informação.");

        var usersLinkedHoteis = await _context.UsuarioPermissao
          .Where(u => u.DetalhesModelId == Guid.Parse(hotelId))
          .Select(x => new GetAllManagers { Email = x.UserModelEmail, Role = x.Role })
          .ToListAsync();

        return Retorno<IEnumerable<GetAllManagers>>.Ok(usersLinkedHoteis, "Lista de usuários com permissão obtida com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<IEnumerable<GetAllManagers>>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno> UpdatePermissionUsers(string hotelId, string email)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Token inválido de e-mail.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado.");

        var hotelGuid = Guid.Parse(hotelId);

        var hotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Id == hotelGuid);

        if (hotel == null)
          return Retorno.Erro("Hotel não encontrado.");

        var userIsAdmin = await _utilsFacade.IsAdminOrManager(user.Id.ToString(), hotelId);

        if (!userIsAdmin)
          return Retorno.Erro("Permissão do usuário não é admin deste hotel.");

        var userManager = await _utilsFacade.GetUserByEmail(email);

        if (userManager == null)
          return Retorno.Erro($"Usuário {email} não encontrado.");

        var isUserManagerAlready = await _utilsFacade.IsAdminOrManager(userManager.Id.ToString(), hotelId);

        if (!isUserManagerAlready)
        {
          var newRole = RoleUserModel.Manager;

          var permissions = new UsuarioPermissoes
          {
            Id = Guid.NewGuid(),
            DetalhesModelId = Guid.Parse(hotelId),
            UserModelEmail = email,
            UserModelId = user.Id,
            Role = newRole
          };
          await _context.UsuarioPermissao.AddAsync(permissions);
          await _context.SaveChangesAsync();
        }

        return Retorno.Ok($"Permissão do usuario {email} atualizada com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno.Excecao(e, "Erro ao processar a solicitação.");
      }
    }
  }
}
