using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Services;

namespace Turify.Filters
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class ValidateHotelAccessAttribute : TypeFilterAttribute
  {
    public ValidateHotelAccessAttribute(bool adminOnly = false)
        : base(typeof(ValidateHotelAccessFilter))
    {
      Arguments = new object[] { adminOnly };
    }
  }

  public class ValidateHotelAccessFilter : IAsyncActionFilter
  {
    private readonly GoogleAuthService _googleAuthService;
    private readonly UtilsFacade _utilsFacade;
    private readonly bool _adminOnly;

    public ValidateHotelAccessFilter(GoogleAuthService googleAuthService, UtilsFacade utilsFacade, bool adminOnly)
    {
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
      _adminOnly = adminOnly;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      // Accepts both {hotelId} and {id} route param names
      var hotelIdStr = context.RouteData.Values.GetValueOrDefault("hotelId")?.ToString()
                    ?? context.RouteData.Values.GetValueOrDefault("id")?.ToString();

      if (!Guid.TryParse(hotelIdStr, out var hotelId))
      {
        context.Result = new ObjectResult(Retorno.Erro("hotel_id inválido.")) { StatusCode = 403 };
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
          ? await _utilsFacade.IsAdminOnly(user.Id, hotelId)
          : await _utilsFacade.IsAdminOrManager(user.Id, hotelId);

      if (!hasPerm)
      {
        context.Result = new ObjectResult(Retorno.Erro("Acesso negado: sem permissão para este hotel.")) { StatusCode = 403 };
        return;
      }

      await next();
    }
  }
}
