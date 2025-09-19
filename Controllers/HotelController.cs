using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using SaudeIA.Data;
using SaudeIA.Facades;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using SaudeIA.Models.Enums;
using System.Data;
using System.Text.Json;

namespace SaudeIA.Controllers
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
      var hoteis = await _hotelFacade.GetAllFacade();
      if (hoteis == null)
        return BadRequest();

      return Ok(hoteis);
    }


    // GET: api/<ValuesController>
    [AllowAnonymous]
    [HttpGet("{hotelId}")]
    public async Task<IActionResult> Get(string hotelId)
    {
      var hotel = await _hotelFacade.GetDetalhesFacade(hotelId);
      if (hotel == null)
        return BadRequest();

      return Ok(hotel);
    }

    // GET: api/<ValuesController>
    [AllowAnonymous]
    [HttpGet("ByManager/{hotelId}")]
    public async Task<IActionResult> GetByManager(string hotelId)
    {
      var hotel = await _hotelFacade.GetDetalhesFacadeByManager(hotelId);
      if (hotel == null)
        return BadRequest();

      return Ok(hotel);
    }

    // GET: api/<ValuesController>
    [AllowAnonymous]
    [HttpGet("ByUserId")]
    public async Task<IActionResult> GetByUserId()
    {
      var hotel = await _hotelFacade.GetDetalhesUserFacade();
      if (hotel == null)
        return BadRequest();

      return Ok(hotel);
    }

    // POST api/<ValuesController>
    [AllowAnonymous]
    [HttpPost()]
    public async Task<IActionResult> Post([FromBody] DetalhesModel obj)
    {
      return await _hotelFacade.PostDetalhesFacade(obj);
    }

    // PUT api/<ValuesController>
    [AllowAnonymous]
    [HttpPut("{hotelId}")]
    public async Task<IActionResult> Put([FromBody] DetalhesModel obj, string hotelId)
    {
      return await _hotelFacade.PutDetalhesFacade(obj, hotelId);
    }

    [AllowAnonymous]
    [HttpPost("{hotelId}/fotos")]
    public async Task<IActionResult> PostFotos(IFormFile file, string hotelId)
    {
      if (file == null || file.Length == 0)
        return BadRequest("Arquivo inválido.");

      var hotel = await _hotelFacade.GetDetalhesFacade(hotelId);

      var bucketName = "hotelaria-imgs";
      var objetoNome = $"{hotel.Name}/{file.FileName}_{Guid.NewGuid()}";
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
    [AllowAnonymous]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      return await _hotelFacade.DeleteDetalhesFacade(id);
    }
  }
}
