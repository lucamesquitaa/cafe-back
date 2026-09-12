using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Services;

namespace Turify.Filters
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class ValidateCafeteriaAccessAttribute : TypeFilterAttribute
  {
    public ValidateCafeteriaAccessAttribute(bool adminOnly = false)
        : base(typeof(ValidateCafeteriaAccessFilter))
    {
      Arguments = new object[] { adminOnly };
    }
  }

  public class ValidateCafeteriaAccessFilter : IAsyncActionFilter
  {
    private readonly GoogleAuthService _googleAuthService;
    private readonly UtilsFacade _utilsFacade;
    private readonly bool _adminOnly;

    public ValidateCafeteriaAccessFilter(GoogleAuthService googleAuthService, UtilsFacade utilsFacade, bool adminOnly)
    {
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
      _adminOnly = adminOnly;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      // Aceita tanto {CafeteriaId} quanto {id} como nome do parâmetro de rota
      var cafeteriaIdStr = context.RouteData.Values.GetValueOrDefault("CafeteriaId")?.ToString()
                    ?? context.RouteData.Values.GetValueOrDefault("id")?.ToString();

      if (!Guid.TryParse(cafeteriaIdStr, out var cafeteriaId))
      {
        context.Result = new ObjectResult(Retorno.Erro("cafeteria_id inválido.")) { StatusCode = 403 };
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

      bool hasPerm = _adminOnly
          ? await _utilsFacade.IsAdminOnly(user.Id, cafeteriaId)
          : await _utilsFacade.IsAdminOrManager(user.Id, cafeteriaId);

      if (!hasPerm)
      {
        context.Result = new ObjectResult(Retorno.Erro("Acesso negado: sem permissão para esta cafeteria.")) { StatusCode = 403 };
        return;
      }

      await next();
    }
  }
}
