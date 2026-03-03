using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Turify.Data;
using Turify.Facades;
using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MotorDeReservasController : ControllerBase
  {
    private readonly Context _context;
    private readonly MotorDeReservasFacade _facade;

    public MotorDeReservasController(Context context, MotorDeReservasFacade facade)
    {
      _context = context;
      _facade = facade;
    }

    [Authorize]
    [HttpPost("{quartoId}/Disponibilidade")]
    public async Task<IActionResult> PostDisponibilidade([FromBody] AddDisponibilidadeDTO disponibilidade, string quartoId)
    {
      var retorno = await _facade.PostDisponibilidadeAsync(disponibilidade, quartoId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [Authorize]
    [HttpPut("{quartoId}/DisponibilidadeDay")]
    public async Task<IActionResult> PutDisponibilidadeDay([FromBody] UpdateDayDisponibilidadeDTO disponibilidadeDay, string quartoId)
    {
      var retorno = await _facade.PutDisponibilidadeDayAsync(disponibilidadeDay, quartoId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [AllowAnonymous]
    [HttpGet("{quartoId}/Disponibilidade")]
    public async Task<IActionResult> GetDisponibilidade(string quartoId)
    {
      var retorno = await _facade.GetDisponibilidadeAsync(quartoId);
      if (retorno == null)
        return BadRequest();
      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [Authorize]
    [HttpPost("{quartoId}/Reserva")]
    public async Task<IActionResult> PostReserva([FromBody] AddReservaDTO reserva, string quartoId)
    {
      var retorno = await _facade.PostReservaAsync(reserva, quartoId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [Authorize]
    [HttpPut("{quartoId}/UpdateReserva")]
    public async Task<IActionResult> UpdateReserva([FromBody] UpdateReservaDTO updatedReserva, string quartoId)
    {
      var retorno = await _facade.PutReservaAsync(updatedReserva, quartoId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [Authorize]
    [HttpGet("{quartoId}/Reserva")]
    public async Task<IActionResult> GetReserva(string quartoId)
    {
      var retorno = await _facade.GetReservaAsync(quartoId);
      if (retorno == null)
        return BadRequest();
      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }
  }
}
