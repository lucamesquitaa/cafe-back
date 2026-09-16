using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cafeteria.Facades;
using Cafeteria.Filters;

namespace Cafeteria.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PhotosController : ControllerBase
  {
    private readonly PhotosFacade _photosFacade;

    public PhotosController(PhotosFacade photosFacade)
    {
      _photosFacade = photosFacade;
    }

    [AllowAnonymous]
    [HttpGet("cafeteria/{CafeteriaId}")]
    public async Task<IActionResult> GetAll(string CafeteriaId)
    {
      var retorno = await _photosFacade.GetAllCafeteriaPhotosAsync(CafeteriaId);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }

    [Authorize]
    [ValidateCafeteriaAccess]
    [HttpPost("cafeteria/{CafeteriaId}")]
    public async Task<IActionResult> Post([FromForm] List<IFormFile> files, string CafeteriaId)
    {
      var retorno = await _photosFacade.PostFotosCafeteriaAsync(files, CafeteriaId);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }

    [Authorize]
    [ValidateCafeteriaAccess]
    [HttpDelete("cafeteria/{CafeteriaId}")]
    public async Task<IActionResult> Delete([FromBody] List<string> imageIds, string CafeteriaId)
    {
      var retorno = await _photosFacade.DeleteImagesAsync(CafeteriaId, imageIds);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);

      return Ok(retorno);
    }
  }
}
