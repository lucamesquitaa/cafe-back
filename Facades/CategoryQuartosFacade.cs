using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.Json;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Services;

namespace Turify.Facades
{
  public class CategoryQuartosFacade : IRetorno
  {
    private readonly Context _context;
    private readonly IRabbitMqProducer _producer;
    private readonly GoogleAuthService _googleAuthService;
    private readonly UtilsFacade _utilsFacade;

    // Implementation of IRetorno properties  
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }

    public CategoryQuartosFacade(Context context, IRabbitMqProducer producer, GoogleAuthService googleAuthService, UtilsFacade utilsFacade)
    {
      _context = context;
      _producer = producer;
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
    }

    public async Task<IRetorno<IEnumerable<CategoryQuarto>>> GetAllCategoryQuartos(string HotelId)
    {
      try
      {
        Guid hotelGuid = Guid.Parse(HotelId); // Validate GUID format

        var categories = await _context.CategoryQuarto.Where(c => c.HotelId == hotelGuid && c.DeletedAt == null).Include(c => c.ConfiguracaoCamas).ToListAsync();

        return Retorno<IEnumerable<CategoryQuarto>>.Ok(categories, $"Dados buscados com sucesso.");
      }
      catch (Exception ex)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + HotelId);
      }
    }

    public async Task<IRetorno<CategoryQuarto>> GetCategoryQuartoById(string id)
    {
      try
      {
        var guid = Guid.Parse(id);

        var categoria = await _context.CategoryQuarto.Include(c => c.ConfiguracaoCamas)
            .FirstOrDefaultAsync(c => c.Id == guid && c.DeletedAt == null);

        return Retorno<CategoryQuarto>.Ok(categoria, $"Dados buscados com sucesso.");

      }
      catch (Exception ex)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + id);
      }
    }

    public async Task<IRetorno> PostCategoryQuartos(CriarCategoryQuartoDTO obj, string HotelId)
    {
      try
      {
        Guid hotelGuid = Guid.Parse(HotelId); // Validate GUID format

        var objTratado = new CategoryQuarto
        {
          Id = Guid.NewGuid(),
          Name = obj.Name,
          MinHospedes = obj.MinHospedes,
          MaxHospedes = obj.MaxHospedes,
          Descricao = obj.Descricao,
          AceitaCamaExtra = obj.AceitaCamaExtra,
          AceitaBerco = obj.AceitaBerco,
          ConfiguracaoCamas = obj.ConfiguracaoCamas,
          HotelId = hotelGuid
        };

        await _context.CategoryQuarto.AddAsync(objTratado);
        await _context.SaveChangesAsync();

        return Retorno.Ok($"Dados cadastrados com sucesso.");
      }
      catch (Exception ex)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + HotelId + " obj: " + JsonSerializer.Serialize(obj) );
      }
    }

    public async Task<IRetorno> PutCategoryQuartos( string id, CategoryQuarto obj)
    {
      try
      {
        var guid = Guid.Parse(id);

        var categoria = await _context.CategoryQuarto
            .FirstOrDefaultAsync(c => c.Id == guid && c.DeletedAt == null);

        if (categoria == null)
          return Retorno<CategoryQuarto>.Erro("Tipo de quarto não encontrado.");

        // Atualiza campos
        categoria.Name = obj.Name;
        categoria.MinHospedes = obj.MinHospedes;
        categoria.MaxHospedes = obj.MaxHospedes;
        categoria.Descricao = obj.Descricao;
        categoria.AceitaCamaExtra = obj.AceitaCamaExtra;
        categoria.AceitaBerco = obj.AceitaBerco;
        categoria.ConfiguracaoCamas = obj.ConfiguracaoCamas;


        await _context.SaveChangesAsync();

        return Retorno<CategoryQuarto>.Ok(categoria);
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao atualizar tipo de quarto: " + e.Message);
      }
    }

    public async Task<IRetorno> DeleteCategoryQuartos(string id)
    {
      try
      {
        Guid idGuid = Guid.Parse(id); // Validate GUID format

        var category = await _context.CategoryQuarto.FirstOrDefaultAsync(x => x.Id == idGuid);

        if(category == null)
          return Retorno.Erro("Categoria não encontrada.");

        // Soft delete do category quarto
        var now = DateTime.UtcNow;
        category.DeletedAt = now;

        await _context.SaveChangesAsync();

        return Retorno.Ok($"Dados cadastrados com sucesso.");
      }
      catch (Exception ex)
      {
        throw new Exception("Erro ao processar a solicitação. id: "  + id);
      }
    }
  }
}
