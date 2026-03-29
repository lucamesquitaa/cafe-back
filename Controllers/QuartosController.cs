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

    [Authorize]
    [ValidateHotelAccess]
    [HttpPost("PostPutQuartos/{hotelId}")]
    public async Task<IActionResult> PostQuartos([FromBody] QuartosModel obj, string hotelId)
    {
      var result = await _quartosFacade.PostQuartosFacade(obj, hotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [Authorize]
    [ValidateHotelAccess]
    [HttpPost("PostQuartosMassa/{hotelId}")]
    public async Task<IActionResult> PostQuartosMassa([FromBody] CriarQuartosMassaDTO dto, string hotelId)
    {
      var result = await _quartosFacade.PostQuartosMassa(hotelId, dto);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [Authorize]
    [ValidateHotelAccess(adminOnly: true)]
    [HttpDelete("DeleteQuarto/{hotelId}/{quartoId}")]
    public async Task<IActionResult> DeleteQuartos(string hotelId, string quartoId)
    {
      var result = await _quartosFacade.DeleteQuartosFacade(hotelId, quartoId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }
  }
}
