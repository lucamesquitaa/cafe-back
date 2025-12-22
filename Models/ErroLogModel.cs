using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turify.Models
{
  public class ErroLogModel
  {
      [Key]
      public int Id { get; set; }
      public string Message { get; set; } = String.Empty;
      public string? StackTrace { get; set; }
      public string Path { get; set; } = String.Empty;
      public string Method { get; set; } = String.Empty;
      public string? UserId { get; set; }
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
  }
