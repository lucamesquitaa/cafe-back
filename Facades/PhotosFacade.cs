using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using Cafeteria.Data;
using Cafeteria.Facades.Interfaces;
using Cafeteria.Models;
using Cafeteria.Models.DTOs;

namespace Cafeteria.Facades
{
  public class PhotosFacade : IPhotosFacade
  {
    private readonly Context _context;
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public PhotosFacade(Context context, IConfiguration configuration)
    {
      _context = context;

      var configuredBucketName = configuration["GoogleCloudStorage:BucketName"];
      _bucketName = !string.IsNullOrWhiteSpace(configuredBucketName)
          ? configuredBucketName
          : Environment.GetEnvironmentVariable("GCS_BUCKET_NAME")
              ?? throw new InvalidOperationException(
                  "Bucket do Google Cloud Storage não configurado. Defina GoogleCloudStorage:BucketName ou a variável de ambiente GCS_BUCKET_NAME.");

      var configuredCredentialsPath = configuration["GoogleCloudStorage:CredentialsPath"];
      var credentialsPath = !string.IsNullOrWhiteSpace(configuredCredentialsPath)
          ? configuredCredentialsPath
          : Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

      _storageClient = string.IsNullOrWhiteSpace(credentialsPath)
          ? StorageClient.Create()
          : StorageClient.Create(GoogleCredential.FromFile(credentialsPath));
    }

    // Implementation of IRetorno properties
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }

    public async Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllCafeteriaPhotosAsync(string cafeteriaId)
    {
      try
      {
        if (!Guid.TryParse(cafeteriaId, out var cafeteriaGuid))
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("Id inválido.");

        var cafeteriaExiste = await _context.Cafeterias.AnyAsync(c => c.Id == cafeteriaGuid);
        if (!cafeteriaExiste)
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("Cafeteria não encontrada.");

        var photos = await _context.Photos
            .AsNoTracking()
            .Where(p => p.CafeteriaId == cafeteriaGuid)
            .Select(p => new GetAllPhotos
            {
              Id = p.Id,
              Alt = p.Alt,
              Url = p.Url,
              Stared = p.Stared
            })
            .ToListAsync();

        return Retorno<IEnumerable<GetAllPhotos>>.Ok(photos, "Fotos buscadas com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + cafeteriaId);
      }
    }

    public async Task<IRetorno<IEnumerable<GetAllPhotos>>> PostFotosCafeteriaAsync(List<IFormFile> files, string cafeteriaId)
    {
      try
      {
        if (files == null || files.Count == 0)
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("Nenhum arquivo enviado.");

        if (!Guid.TryParse(cafeteriaId, out var cafeteriaGuid))
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("Id inválido.");

        var cafeteria = await _context.Cafeterias.FirstOrDefaultAsync(c => c.Id == cafeteriaGuid);
        if (cafeteria == null)
          return Retorno<IEnumerable<GetAllPhotos>>.Erro("Cafeteria não encontrada.");

        var photos = new List<FotosDetalhesModel>();

        foreach (var file in files)
        {
          if (file.Length == 0)
            continue;

          var fileId = Guid.NewGuid();
          var fileName = $"IMG-{fileId}{Path.GetExtension(file.FileName)}";
          var objectName = $"{cafeteria.Nome}/{fileName}";

          using var stream = file.OpenReadStream();

          await _storageClient.UploadObjectAsync(
              bucket: _bucketName,
              objectName: objectName,
              contentType: file.ContentType,
              source: stream);

          var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";

          var img = new FotosDetalhesModel
          {
            Id = fileId,
            CafeteriaId = cafeteriaGuid,
            Alt = objectName,
            Url = publicUrl,
            Stared = false
          };

          photos.Add(img);
          await _context.Photos.AddAsync(img);
        }

        await _context.SaveChangesAsync();

        var resultado = photos.Select(p => new GetAllPhotos
        {
          Id = p.Id,
          Alt = p.Alt,
          Url = p.Url,
          Stared = p.Stared
        });

        return Retorno<IEnumerable<GetAllPhotos>>.Ok(resultado, "Fotos enviadas com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação de upload de fotos.");
      }
    }

    public async Task<IRetorno<string>> UploadFotoPrincipalAsync(IFormFile file, string cafeteriaNome)
    {
      try
      {
        if (file == null || file.Length == 0)
          return Retorno<string>.Erro("Nenhum arquivo enviado.");

        var fileName = $"IMG-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var objectName = $"{cafeteriaNome}/Principal/{fileName}";

        using var stream = file.OpenReadStream();

        await _storageClient.UploadObjectAsync(
            bucket: _bucketName,
            objectName: objectName,
            contentType: file.ContentType,
            source: stream);

        var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";

        return Retorno<string>.Ok(publicUrl, "Foto principal enviada com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação de upload da foto principal.");
      }
    }

    public async Task<IRetorno> DeleteImagesAsync(string cafeteriaId, List<string> imageIds)
    {
      try
      {
        if (!Guid.TryParse(cafeteriaId, out var cafeteriaGuid))
          return Retorno.Erro("Id inválido.");

        if (imageIds == null || imageIds.Count == 0)
          return Retorno.Erro("Nenhuma imagem informada.");

        foreach (var imageId in imageIds)
        {
          if (!Guid.TryParse(imageId, out var photoGuid))
            continue;

          var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == photoGuid && p.CafeteriaId == cafeteriaGuid);
          if (photo == null)
            continue;

          await _storageClient.DeleteObjectAsync(_bucketName, photo.Alt);
          _context.Photos.Remove(photo);
        }

        await _context.SaveChangesAsync();
        return Retorno.Ok(mensagem: "Imagens deletadas com sucesso.");
      }
      catch (Exception ex)
      {
        return Retorno.Erro($"Erro ao deletar imagens: {ex.Message}");
      }
    }
  }
}
