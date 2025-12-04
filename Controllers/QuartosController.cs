using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using SaudeIA.Data;
using SaudeIA.Facades;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using SaudeIA.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaudeIA.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class QuartosController : ControllerBase
  {
    private readonly QuartosFacade _quartosFacade;

    public QuartosController(QuartosFacade quartosFacade)
    {
      _quartosFacade = quartosFacade;
    }

    [AllowAnonymous]
    [HttpGet("GetAllQuartos/{hotelId}")]
    public async Task<IActionResult> GetAllQuartos(string hotelId)
    {
       var result = await _quartosFacade.GetAllQuartos(hotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("GetQuartoById/{quartoId}")]
    public async Task<IActionResult> GetQuartoById(string quartoId)
    {
      var result = await _quartosFacade.GetQuartoById(quartoId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [HttpPost("PostPutQuartos/{hotelId}")]
    public async Task<IActionResult> PostQuartos([FromBody] QuartosModel obj, string hotelId)
    {
      var result = await _quartosFacade.PostQuartosFacade(obj, hotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }
  }
}
