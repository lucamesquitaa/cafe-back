using Microsoft.AspNetCore.Mvc;
using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Facades.Interfaces
{
  public interface IPhotosFacade : IRetorno
  {
    Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllHotelPhotosAsync(string HotelId);
    Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllQuartosPhotosAsync(string HotelId);
    Task<IRetorno> PostFotosHotelAsync([FromForm] List<IFormFile> files, string HotelId, string? quartoId);
    Task<IRetorno> DeleteImagesAsync([FromBody] List<string> imageIds);
  }
}
