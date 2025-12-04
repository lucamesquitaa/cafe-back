using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SaudeIA.Data;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Services;

namespace SaudeIA.Facades
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

    public async Task<IRetorno<IEnumerable<CategoryQuarto>>> GetAllCategoryQuartos(string hotelId)
    {
      try
      {
        Guid hotelGuid = Guid.Parse(hotelId); // Validate GUID format

        var categories = await _context.CategoryQuarto.Where(c => c.DetalhesModelId == hotelGuid).ToListAsync();

        return Retorno<IEnumerable<CategoryQuarto>>.Ok(categories, $"Dados buscados com sucesso.");
      }
      catch (Exception ex)
      {
        return Retorno<IEnumerable<CategoryQuarto>>.Excecao(ex, "Erro ao processar a solicitação.");
      }
    }
    public async Task<IRetorno> PostCategoryQuartos(CategoryQuarto obj, string hotelId)
    {
      try
      {
        Guid hotelGuid = Guid.Parse(hotelId); // Validate GUID format

        obj.DetalhesModelId = hotelGuid;

        await _context.CategoryQuarto.AddAsync(obj);
        await _context.SaveChangesAsync();

        return Retorno.Ok($"Dados cadastrados com sucesso.");
      }
      catch (Exception ex)
      {
        return Retorno.Excecao(ex, "Erro ao processar a solicitação.");
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

        _context.CategoryQuarto.Remove(category);
        await _context.SaveChangesAsync();

        return Retorno.Ok($"Dados cadastrados com sucesso.");
      }
      catch (Exception ex)
      {
        return Retorno.Excecao(ex, "Erro ao processar a solicitação.");
      }
    }
  }
}
