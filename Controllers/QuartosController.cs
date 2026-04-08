using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using Turify.Data;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Filters;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Turify.Controllers
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
    [HttpGet("GetAllQuartos/{HotelId}")]
    public async Task<IActionResult> GetAllQuartos(string HotelId)
    {
       var result = await _quartosFacade.GetAllQuartos(HotelId);

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

    [Authorize]
    [ValidateHotelAccess]
    [HttpPost("PostPutQuartos/{HotelId}")]
    public async Task<IActionResult> PostQuartos([FromBody] QuartosModel obj, string HotelId)
    {
      var result = await _quartosFacade.PostQuartosFacade(obj, HotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [Authorize]
    [ValidateHotelAccess]
    [HttpPost("PostQuartosMassa/{HotelId}")]
    public async Task<IActionResult> PostQuartosMassa([FromBody] CriarQuartosMassaDTO dto, string HotelId)
    {
      var result = await _quartosFacade.PostQuartosMassa(HotelId, dto);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [Authorize]
    [ValidateHotelAccess(adminOnly: true)]
    [HttpDelete("DeleteQuarto/{HotelId}/{quartoId}")]
    public async Task<IActionResult> DeleteQuartos(string HotelId, string quartoId)
    {
      var result = await _quartosFacade.DeleteQuartosFacade(HotelId, quartoId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }
  }
}
