using Microsoft.EntityFrameworkCore;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.Enums;
using Turify.Services;

namespace Turify.Facades
{
  public class UtilsFacade
  {
    private readonly Context _context;

    public UtilsFacade(Context context)
    {
      _context = context;
    }
    public async Task<UserModel?> GetUserByEmail(string email)
    {
      return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsAdminOrManager(Guid userId, Guid hotelId)
    {
      var user = await _context.UsuarioPermissao.FirstOrDefaultAsync(x => x.UserModelId == userId && x.DetalhesModelId == hotelId);

      bool userIsAdmin = user?.Role == RoleUserModel.Admin || user?.Role == RoleUserModel.Manager || user?.Role == RoleUserModel.Turify;

      return userIsAdmin;
    }

    public async Task<bool> IsAdminOnly(Guid userId, Guid hotelId)
    {
      var user = await _context.UsuarioPermissao.FirstOrDefaultAsync(x => x.UserModelId == userId && x.DetalhesModelId == hotelId);

      bool userIsAdmin = user?.Role == RoleUserModel.Admin || user?.Role == RoleUserModel.Turify;

      return userIsAdmin;
    }
  }
}
