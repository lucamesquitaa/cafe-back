using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turify.Models.DTOs
{
  public class CriarCategoryQuartoDTO
  {
    public string Name { get; set; }
    public int? MinHospedes { get; set; }
    public int? MaxHospedes { get; set; }
    public string? Descricao { get; set; }
    public bool AceitaCamaExtra { get; set; }
    public bool AceitaBerco { get; set; }
    public List<BedsDTO> ConfiguracaoCamas { get; set; } = new List<BedsDTO>();
  }
}
