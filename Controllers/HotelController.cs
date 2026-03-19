using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Turify.Data;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Filters;
using Turify.Models;

namespace Turify.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class HotelController : ControllerBase
  {
    private readonly HotelFacade _hotelFacade;

    public HotelController(HotelFacade hotelFacade)
    {
      _hotelFacade = hotelFacade;
    }


    // GET: api/<ValuesController>
    [AllowAnonymous]
    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
      var retorno = await _hotelFacade.GetAllFacade();

      if (retorno.Sucesso == false && !string.IsNullOrEmpty(retorno.ExcecaoMensagem))
        return StatusCode(500, retorno);
      else if(retorno.Sucesso == false && string.IsNullOrEmpty(retorno.ExcecaoMensagem))
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }


    // GET: api/<ValuesController>
    [AllowAnonymous]
    [HttpGet("{hotelId}")]
    public async Task<IActionResult> Get(string hotelId)
    {
      var retorno = await _hotelFacade.GetDetalhesFacade(hotelId);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }

    // GET: api/<ValuesController>
    [Authorize]
    [ValidateHotelAccess]
    [HttpGet("ByManager/{hotelId}")]
    public async Task<IActionResult> GetByManager(string hotelId)
    {
      var retorno = await _hotelFacade.GetDetalhesFacadeByManager(hotelId);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }

    // GET: api/<ValuesController>
    [Authorize]
    [HttpGet("ByUserId")]
    public async Task<IActionResult> GetByUserId()
    {
      var retorno = await _hotelFacade.GetDetalhesUserFacade();
        
      if (retorno.Sucesso == false)
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }

    // POST api/<ValuesController>
    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> Post([FromBody] DetalhesModel obj)
    {
      var retorno = await _hotelFacade.PostDetalhesFacade(obj);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }

    // PUT api/<ValuesController>
    [Authorize]
    [ValidateHotelAccess]
    [HttpPut("{hotelId}")]
    public async Task<IActionResult> Put([FromBody] DetalhesModel obj, string hotelId)
    {
      var retorno = await _hotelFacade.PutDetalhesFacade(obj, hotelId);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }

    [Authorize]
    [ValidateHotelAccess]
    [HttpPost("{hotelId}/fotos")]
    public async Task<IActionResult> PostFotos(IFormFile file, string hotelId)
    {
      if (file == null || file.Length == 0)
        return BadRequest("Arquivo inválido.");

      var retorno = await _hotelFacade.GetDetalhesFacade(hotelId);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);   

      var bucketName = "turify-imgs";
      var objetoNome = $"{retorno.Data.Name}/{file.FileName}_{Guid.NewGuid()}";
      // Busca o segredo no Secret Manager
      //HOMOLOG
      //var secretClient = await SecretManagerServiceClient.CreateAsync();
      //var secretName = new SecretVersionName("just-stock-461116-u2", "service-account", "latest");
      //var secret = await secretClient.AccessSecretVersionAsync(secretName);
      //var secretForm = secret.Payload.Data.ToByteArray();


      var jsonPath = Path.Combine(AppContext.BaseDirectory, "HOMOLOG", "just-stock-461116-u2-0ef74d61db10.json");
      var credential = GoogleCredential.FromFile(jsonPath);
      //await System.IO.File.WriteAllBytesAsync(tempPath, secretForm);

      // Autentica usando o JSON baixado
      //var credential = GoogleCredential.FromFile(tempPath);
      var storage = await StorageClient.CreateAsync(credential);

      using (var stream = file.OpenReadStream())
      {
        await storage.UploadObjectAsync(bucketName, objetoNome, file.ContentType, stream);
      }

      var publicUrl = $"https://storage.googleapis.com/{bucketName}/{objetoNome}";
      return Ok(new { url = publicUrl });
    }

    // DELETE api/<ValuesController>/5
    [Authorize]
    [ValidateHotelAccess(adminOnly: true)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      var retorno = await _hotelFacade.DeleteDetalhesFacade(id);

      if (retorno.Sucesso == false)
        return BadRequest(retorno);
      else
        return Ok(retorno);
    }
  }
}
