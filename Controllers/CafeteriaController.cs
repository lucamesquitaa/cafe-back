using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cafeteria.Facades;
using Cafeteria.Filters;
using Cafeteria.Models.DTOs;

namespace Cafeteria.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CafeteriasController : ControllerBase
  {
    private readonly CafeteriaFacade _cafeteriaFacade;

    public CafeteriasController(CafeteriaFacade cafeteriaFacade)
    {
      _cafeteriaFacade = cafeteriaFacade;
    }

    [AllowAnonymous]
    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
      var retorno = await _cafeteriaFacade.GetAllFacade(page, pageSize);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      var retorno = await _cafeteriaFacade.GetByIdFacade(id);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> Post([FromBody] CriarCafeteriaDTO obj)
    {
      var retorno = await _cafeteriaFacade.PostFacade(obj);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }

    [Authorize]
    [ValidateCafeteriaAccess]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromBody] AtualizarCafeteriaDTO obj, string id)
    {
      var retorno = await _cafeteriaFacade.PutFacade(obj, id);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }
  }
}
