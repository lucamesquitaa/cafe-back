using Cafeteria.Models.DTOs;

namespace Cafeteria.Facades.Interfaces
{
  public interface IPhotosFacade : IRetorno
  {
    Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllCafeteriaPhotosAsync(string cafeteriaId);
    Task<IRetorno<IEnumerable<GetAllPhotos>>> PostFotosCafeteriaAsync(List<IFormFile> files, string cafeteriaId);
    Task<IRetorno<string>> UploadFotoPrincipalAsync(IFormFile file, string cafeteriaNome);
    Task<IRetorno> DeleteImagesAsync(string cafeteriaId, List<string> imageIds);
  }
}
