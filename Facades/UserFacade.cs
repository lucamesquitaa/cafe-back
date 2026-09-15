using Microsoft.EntityFrameworkCore;
using Cafeteria.Data;
using Cafeteria.Facades.Interfaces;
using Cafeteria.Models;
using Cafeteria.Models.DTOs;
using Cafeteria.Models.Enums;
using Cafeteria.Services;

namespace Cafeteria.Facades
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
        if (userGoogle == null || string.IsNullOrWhiteSpace(userGoogle.IdToken))
          return Retorno<UserModel>.Erro("Token do Google não informado.");

        var payload = await _googleAuthService.ValidateIdTokenAsync(userGoogle.IdToken);

        if (payload == null)
          return Retorno<UserModel>.Erro("Token do Google inválido ou expirado.");

        if (string.IsNullOrWhiteSpace(payload.Email))
          return Retorno<UserModel>.Erro("Token do Google não contém e-mail.");

        if (!payload.EmailVerified)
          return Retorno<UserModel>.Erro("E-mail do Google não verificado.");

        var googleId = payload.Subject;
        var email = payload.Email.Trim().ToLowerInvariant();
        var firstName = !string.IsNullOrWhiteSpace(payload.GivenName) ? payload.GivenName : payload.Name;
        var lastName = payload.FamilyName;
        var picture = payload.Picture;

        var user = await _utilsFacade.GetUserByEmail(email);

        if (user == null)
        {
          user = new UserModel
          {
            Id = Guid.NewGuid(),
            GoogleId = googleId,
            FirstName = firstName ?? string.Empty,
            LastName = lastName,
            Email = email,
            Photo = picture
          };
          _context.Usuarios.Add(user);
        }
        else
        {
          user.GoogleId = googleId;
          user.FirstName = firstName ?? user.FirstName;
          user.LastName = lastName ?? user.LastName;
          user.Photo = picture ?? user.Photo;
        }

        await _context.SaveChangesAsync();

        return Retorno<UserModel>.Ok(user, "Login ou registro realizado com sucesso.");
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao processar a solicitação de login com Google.", e);
      }
    }

    public async Task<IRetorno<IEnumerable<GetAllManagers>>> GetAllPermissionUsers(string CafeteriaId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Erro ao buscar usuário.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Erro ao buscar usuário.");

        var cafeteriaIdGuid = Guid.Parse(CafeteriaId);

        var cafeteria = await _context.Cafeterias.FirstOrDefaultAsync(u => u.Id == cafeteriaIdGuid);

        if (cafeteria == null)
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Erro ao buscar cafeteria.");

        var userIsAdmin = await _utilsFacade.IsAdminOrManager(user.Id, cafeteriaIdGuid);

        if (!userIsAdmin)
          return Retorno<IEnumerable<GetAllManagers>>.Erro("Usuário não possui permissão para acessar esta informação.");

        var usersLinkedCafeterias = await _context.UsuarioPermissao
          .Where(u => u.CafeteriaId == cafeteriaIdGuid)
          .Select(x => new GetAllManagers { Email = x.UserModelEmail, Role = x.Role })
          .ToListAsync();

        return Retorno<IEnumerable<GetAllManagers>>.Ok(usersLinkedCafeterias, "Lista de usuários com permissão obtida com sucesso.");
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + CafeteriaId);
      }
    }

    public async Task<IRetorno> UpdatePermissionUsers(string CafeteriaId, string email)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Token inválido de e-mail.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado.");

        var cafeteriaIdGuid = Guid.Parse(CafeteriaId);

        var cafeteria = await _context.Cafeterias.FirstOrDefaultAsync(u => u.Id == cafeteriaIdGuid);

        if (cafeteria == null)
          return Retorno.Erro("Cafeteria não encontrada.");

        var userIsAdmin = await _utilsFacade.IsAdminOnly(user.Id, cafeteriaIdGuid);

        if (!userIsAdmin)
          return Retorno.Erro("Permissão do usuário não é admin desta cafeteria.");

        var userManager = await _utilsFacade.GetUserByEmail(email);

        if (userManager == null)
          return Retorno.Erro($"Usuário {email} não encontrado.");

        var isUserManagerAlready = await _utilsFacade.IsAdminOrManager(userManager.Id, cafeteriaIdGuid);

        if (isUserManagerAlready)
          return Retorno.Erro($"Usuário {email} já possui permissão de manager nesta cafeteria.");

        if (!isUserManagerAlready)
        {
          var newRole = RoleUserModel.Manager;

          var permissions = new UsuarioPermissoes
          {
            Id = Guid.NewGuid(),
            CafeteriaId = cafeteriaIdGuid,
            UserModelEmail = email,
            UserModelId = userManager.Id,
            Role = newRole
          };
          await _context.UsuarioPermissao.AddAsync(permissions);
          await _context.SaveChangesAsync();
        }

        return Retorno.Ok($"Permissão do usuario {email} atualizada com sucesso.");
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + CafeteriaId + "obj: " + email);
      }
    }

    public async Task<IRetorno> RemovePermissionUsers(string CafeteriaId, string email)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Token inválido de e-mail.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado.");

        var cafeteriaIdGuid = Guid.Parse(CafeteriaId);

        var cafeteria = await _context.Cafeterias.FirstOrDefaultAsync(u => u.Id == cafeteriaIdGuid);

        if (cafeteria == null)
          return Retorno.Erro("Cafeteria não encontrada.");

        var userIsAdmin = await _utilsFacade.IsAdminOnly(user.Id, cafeteriaIdGuid);

        if (!userIsAdmin)
          return Retorno.Erro("Permissão do usuário não é admin desta cafeteria.");

        var userManager = await _utilsFacade.GetUserByEmail(email);

        if (userManager == null)
          return Retorno.Erro($"Usuário {email} não encontrado.");

          var permissions = await _context.UsuarioPermissao
                                          .FirstOrDefaultAsync(u => u.CafeteriaId == cafeteriaIdGuid && u.UserModelId == userManager.Id);

          _context.UsuarioPermissao.Remove(permissions);
          await _context.SaveChangesAsync();


        return Retorno.Ok($"Permissão do usuario {email} removida com sucesso.");
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + CafeteriaId + "obj: " + email);
      }
    }
  }
}
