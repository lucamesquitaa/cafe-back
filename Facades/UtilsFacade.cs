using Microsoft.EntityFrameworkCore;
using Cafeteria.Data;
using Cafeteria.Facades.Interfaces;
using Cafeteria.Models;
using Cafeteria.Models.Enums;
using Cafeteria.Services;

namespace Cafeteria.Facades
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

    public async Task<bool> IsAdminOrManager(Guid userId, Guid cafeteriaId)
    {
      var user = await _context.UsuarioPermissao.FirstOrDefaultAsync(x => x.UserModelId == userId && x.CafeteriaId == cafeteriaId);

      bool userIsAdmin = user?.Role == RoleUserModel.Admin || user?.Role == RoleUserModel.Manager || user?.Role == RoleUserModel.Cafeteria;

      return userIsAdmin;
    }

    public async Task<bool> IsAdminOnly(Guid userId, Guid cafeteriaId)
    {
      var user = await _context.UsuarioPermissao.FirstOrDefaultAsync(x => x.UserModelId == userId && x.CafeteriaId == cafeteriaId);

      bool userIsAdmin = user?.Role == RoleUserModel.Admin || user?.Role == RoleUserModel.Cafeteria;

      return userIsAdmin;
    }
  }
}
