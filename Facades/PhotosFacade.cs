using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Security.AccessControl;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Services;

namespace Turify.Facades
{
  public class PhotosFacade : IPhotosFacade
  {
    private readonly Context _context;
    private readonly GoogleAuthService _googleAuthService;
    private UtilsFacade _utilsFacade;
    private readonly StorageClient _storageClient;
    private const string BucketName = "turify-imgs";

    public PhotosFacade(Context context, GoogleAuthService googleAuthService, UtilsFacade utilsFacade)
    {
      _context = context;
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
      var sec =  GoogleCredential.FromFile("HOMOLOG/clinica-278816-4bf203a74dfd.json");
      _storageClient = StorageClient.Create(sec);
    }

    // Implementation of IRetorno properties  
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }

    public async Task<IRetorno> PostFotosHotelAsync([FromForm] List<IFormFile> files, string HotelId, string? quartoId)
    {
      try
      {
        
        // Implementation logic to add photos to hotel
        if (files == null)
          return Retorno.Erro("Nenhum arquivo enviado.");


        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado - id.");

        Guid hotelGuid = new Guid(HotelId);

        var hotelName = await _context.Hotel.FirstOrDefaultAsync(u => u.Id == hotelGuid);
        if (hotelName == null || string.IsNullOrEmpty(hotelName?.Name))
          return Retorno.Erro("HotelId não encontrado.");

        var photos = new List<FotosDetalhesModel>();

        //fotos para o quarto ? 
        if (quartoId != null || !string.IsNullOrEmpty(quartoId))
        {
          Guid quartoGuid = new Guid(quartoId);
          var quarto = await _context.Quartos.FirstOrDefaultAsync(q => q.Id == quartoGuid);
          if (quarto == null)
            return Retorno.Erro("QuartoId não encontrado para o hotel informado.");

          

          foreach (var file in files)
          {
            if (file.Length == 0)
              continue;

            var fileId = Guid.NewGuid();

            var fileName = $"IMG-{fileId}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();

            string objectName = $@"{hotelName.Name}/{quarto.Name}/{fileName}";

            var uploadOptions = new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead };
            // URL pública permanente
            var publicUrl = $"https://storage.googleapis.com/{BucketName}/{objectName}";

            await _storageClient.UploadObjectAsync(
                bucket: BucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: stream
            );


            FotosDetalhesModel img = new FotosDetalhesModel
            {
              Id = fileId,
              RoomId = quartoGuid,
              Alt = objectName,
              Url = publicUrl,
              Stared = false
            };

            photos.Add(img);

            await _context.Photos.AddAsync(img);

          }

        }
        else //fotos para o hotel
        {


          foreach (var file in files)
          {
            if (file.Length == 0)
              continue;

            var fileId = Guid.NewGuid();

            var fileName = $"IMG-{fileId}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();

            string objectName = $@"{hotelName.Name}/{fileName}";

            var uploadOptions = new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead };
            // URL pública permanente
            var publicUrl = $"https://storage.googleapis.com/{BucketName}/{objectName}";

            await _storageClient.UploadObjectAsync(
                bucket: BucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: stream
            );


            FotosDetalhesModel img = new FotosDetalhesModel
            {
              Id = fileId,
              HotelId = hotelGuid,
              Alt = objectName,
              Url = publicUrl,
              Stared = false
            };

            photos.Add(img);

            await _context.Photos.AddAsync(img);

          }
        }
        
        await _context.SaveChangesAsync();
        return Retorno.Ok(photos);

      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<IRetorno> DeleteImagesAsync(List<string> imageIds)
    {
      try
      {
        foreach (var imageId in imageIds)
        {
          Guid guid = new Guid(imageId);

          var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == guid);
          if (photo != null)
          {
            await _storageClient.DeleteObjectAsync(BucketName, photo.Alt);
            _context.Photos.Remove(photo);
          }
        }
        await _context.SaveChangesAsync();
        return Retorno.Ok("Imagens deletadas com sucesso.");
      }
      catch (Exception ex)
      {
        return Retorno.Erro($"Erro ao deletar imagens: {ex.Message}");
      }
    }

    public async Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllHotelPhotosAsync(string HotelId)
    {
      try
      {
        Guid hotelGuid = new Guid(HotelId);

        var hotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Id == hotelGuid);

        if (hotel == null)
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("HotelId não encontrado.");

        var photos = await _context.Photos
            .Where(p => p.HotelId == hotelGuid)
            .Select(p => new GetAllPhotos
            {
              Id = p.Id,
              Alt = p.Alt,
              Url = p.Url,
              Stared = p.Stared
            })
            .ToListAsync();

        return Retorno<IEnumerable<GetAllPhotos>>.Ok(photos);

      }
      catch (Exception e)
      {
        throw new Exception(e.Message);
      }
    }

    public async Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllQuartosPhotosAsync(string quartoId)
    {
      try
      {
        Guid quartoGuid = new Guid(quartoId);

        var hotel = await _context.Quartos.FirstOrDefaultAsync(u => u.Id == quartoGuid);

        if (hotel == null)
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("QuartoId não encontrado.");

        var photos = await _context.Photos
            .Where(p => p.RoomId == quartoGuid)
            .Select(p => new GetAllPhotos
            {
              Id = p.Id,
              Alt = p.Alt,
              Url = p.Url,
              Stared = p.Stared
            })
            .ToListAsync();

        return Retorno<IEnumerable<GetAllPhotos>>.Ok(photos);

      }
      catch (Exception e)
      {
        throw new Exception(e.Message);
      }
    }
  }
}
