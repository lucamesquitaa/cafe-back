using Microsoft.AspNetCore.Mvc;
using Turify.Models;
using Turify.Models.DTOs;

namespace Turify.Facades.Interfaces
{
  public interface IPhotosFacade : IRetorno
  {
    Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllHotelPhotosAsync(string hotelId);
    Task<IRetorno<IEnumerable<GetAllPhotos>>> GetAllQuartosPhotosAsync(string hotelId);
    Task<IRetorno> PostFotosHotelAsync([FromForm] List<IFormFile> files, string hotelId, string? quartoId);
    Task<IRetorno> DeleteImagesAsync([FromBody] List<string> imageIds);
  }
}
