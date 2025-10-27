using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using SaudeIA.Models.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace SaudeIA.Services
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
          Audience = new[] { "54700728866-d81aiuid54e66ju93oia449v9dkcs357.apps.googleusercontent.com" } // garante que o token é para seu app
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
