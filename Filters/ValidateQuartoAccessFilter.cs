using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Turify.Data;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Services;

namespace Turify.Filters
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class ValidateQuartoAccessAttribute : TypeFilterAttribute
  {
    public ValidateQuartoAccessAttribute()
        : base(typeof(ValidateQuartoAccessFilter))
    {
    }
  }

  public class ValidateQuartoAccessFilter : IAsyncActionFilter
  {
    private readonly GoogleAuthService _googleAuthService;
    private readonly UtilsFacade _utilsFacade;
    private readonly Context _context;

    public ValidateQuartoAccessFilter(GoogleAuthService googleAuthService, UtilsFacade utilsFacade, Context context)
    {
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
      _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      if (!context.RouteData.Values.TryGetValue("quartoId", out var quartoIdObj)
          || !Guid.TryParse(quartoIdObj?.ToString(), out var quartoId))
      {
        context.Result = new ObjectResult(Retorno.Erro("quartoId inválido.")) { StatusCode = 403 };
        return;
      }

      var quarto = await _context.Quartos.AsNoTracking().FirstOrDefaultAsync(q => q.Id == quartoId);
      if (quarto == null)
      {
        context.Result = new ObjectResult(Retorno.Erro("Quarto não encontrado.")) { StatusCode = 403 };
        return;
      }

      var userEmail = _googleAuthService.GetUserEmailFromToken();
      if (string.IsNullOrEmpty(userEmail))
      {
        context.Result = new ObjectResult(Retorno.Erro("Usuário não autorizado.")) { StatusCode = 403 };
        return;
      }

      var user = await _utilsFacade.GetUserByEmail(userEmail);
      if (user == null || user.Id == Guid.Empty)
      {
        context.Result = new ObjectResult(Retorno.Erro("Usuário não autorizado.")) { StatusCode = 403 };
        return;
      }

      bool hasPerm = await _utilsFacade.IsAdminOrManager(user.Id, quarto.DetalhesModelId);
      if (!hasPerm)
      {
        context.Result = new ObjectResult(Retorno.Erro("Acesso negado: sem permissão para este hotel.")) { StatusCode = 403 };
        return;
      }

      await next();
    }
  }
}
