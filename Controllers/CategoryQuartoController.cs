using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CategoryQuartoController : Controller
  {
    private readonly CategoryQuartosFacade _ctQuartoFacade;

    public CategoryQuartoController(CategoryQuartosFacade ctQuartoFacade)
    {
      _ctQuartoFacade = ctQuartoFacade;
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> Get(string hotelId)
    {
      var result = await _ctQuartoFacade.GetAllCategoryQuartos(hotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
      try
      {
        var result = await _ctQuartoFacade.GetCategoryQuartoById(id);

        if (result == null)
          return BadRequest("Tipo de quarto não encontrado.");

        return Ok(result);
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao buscar tipo de quarto: " + e.Message);
      }
    }

    // POST: CategoryQuartoController/Edit/5
    [HttpPost("PostCategoryQuarto/{hotelId}")]
    public async Task<IActionResult> PostQuartos([FromBody] CriarCategoryQuartoDTO obj, string hotelId)
    {
      var result = await _ctQuartoFacade.PostCategoryQuartos(obj, hotelId);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);

    }

    [HttpPut("PutCategoryQuarto/{id}")]
    [Authorize]
    public async Task<IActionResult> PutCategoryQuarto(
    string id, [FromBody] CategoryQuarto dto)
    {
      var result = await _ctQuartoFacade.PutCategoryQuartos(id, dto);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);
    }


    // GET: CategoryQuartoController/Delete/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuartos(string id)
    {
      var result = await _ctQuartoFacade.DeleteCategoryQuartos(id);

      if (result.Sucesso == false)
        return BadRequest(result);
      else
        return Ok(result);

    }
  }
}
