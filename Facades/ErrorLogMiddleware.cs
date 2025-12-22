using Turify.Data;
using Turify.Models;
using System;
using System.Reflection;

namespace Turify.Facades
{
  public class ErrorLoggingMiddleware
  {
    private readonly RequestDelegate _next;

    public ErrorLoggingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, Context db)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        var log = new ErroLogModel
        {
          Message = ex.Message,
          StackTrace = ex.StackTrace,
          Path = context.Request.Path,
          Method = context.Request.Method,
          UserId = context.User?.Identity?.Name,
        };

        db.ErrorLogs.Add(log);
        await db.SaveChangesAsync();

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
          error = "Erro interno, já registrado no sistema."
        });
      }
    }
  }

}
