using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Turify.Models.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace Turify.Services
{
  public class GoogleAuthService
  {
    private readonly string _googleClientId;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GoogleAuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
      _googleClientId = configuration["Authentication:Google:ClientId"];
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GoogleJsonWebSignature.Payload?> ValidateIdTokenAsync(string idToken)
    {
      try
      {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
          Audience = new[] { _googleClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        return payload;
      }
      catch (InvalidJwtException ex)
      {
        Console.WriteLine($"Token inválido: {ex.Message}");
        return null;
      }
    }

    public string? GetUserEmailFromToken()
    {
      // 1. Captura o token do cabeçalho Authorization
      var httpContext = _httpContextAccessor.HttpContext;
      var authHeader = httpContext?.Request.Headers["Authorization"].ToString();

      if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        return null;

      var token = authHeader.Substring("Bearer ".Length);

      if (string.IsNullOrEmpty(token))
        return null;

      var handler = new JwtSecurityTokenHandler();
      if (handler.CanReadToken(token))
      {
        var jwtToken = handler.ReadJwtToken(token);
        var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        return sub;
      }
      return null;
    }
  }
}
