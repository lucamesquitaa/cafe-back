namespace Turify.Models.DTOs
{
  public class GetAllPhotos
  {
    public Guid Id { get; set; }
    public string Alt { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool? Stared { get; set; }
  }
}
