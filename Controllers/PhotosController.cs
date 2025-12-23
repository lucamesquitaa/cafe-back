using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Turify.Data;
using Turify.Facades;

namespace Turify.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PhotosController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly Context _context;
    private readonly PhotosFacade _facade;

    public PhotosController(IConfiguration configuration, Context context, PhotosFacade facade)
    {
      _configuration = configuration;
      _context = context;
      _facade = facade;
    }

    [Authorize]
    [HttpPost("{hotelId}/fotos")]
    public async Task<IActionResult> PostFotos([FromForm] List<IFormFile> files, [FromQuery(Name = "quartoId")]string? quartoId, string hotelId)
    {
      var retorno = await _facade.PostFotosHotelAsync(files, hotelId, quartoId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [Authorize]
    [HttpDelete()]
    public async Task<IActionResult> DeleteFotos([FromBody]List<string> imageIds)
    {
      var retorno = await _facade.DeleteImagesAsync(imageIds);

      if(retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [AllowAnonymous]
    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetAllHotelPhotos(string hotelId)
    {
      var retorno = await _facade.GetAllHotelPhotosAsync(hotelId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [AllowAnonymous]
    [HttpGet("Quarto/{quartoId}")]
    public async Task<IActionResult> GetAllQuartosPhotos(string quartoId)
    {
      var retorno = await _facade.GetAllQuartosPhotosAsync(quartoId);

      if (retorno == null)
        return BadRequest();

      return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }
  }
}
