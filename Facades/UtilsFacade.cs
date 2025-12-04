using Microsoft.EntityFrameworkCore;
using SaudeIA.Data;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.Enums;
using SaudeIA.Services;

namespace SaudeIA.Facades
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
  }
}
