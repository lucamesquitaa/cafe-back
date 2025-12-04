using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaudeIA.Facades;
using SaudeIA.Models;

namespace SaudeIA.Controllers
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

    // POST: CategoryQuartoController/Edit/5
    [HttpPost("PostCategoryQuarto/{hotelId}")]
    public async Task<IActionResult> PostQuartos([FromBody] CategoryQuarto obj, string hotelId)
    {
      var result = await _ctQuartoFacade.PostCategoryQuartos(obj, hotelId);

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
