using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaudeIA.Data;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using SaudeIA.Models.Enums;
using SaudeIA.Services;

namespace SaudeIA.Facades
{
  public class QuartosFacade : IQuartosFacade, IRetorno
  {
    private readonly Context _context;
    private readonly GoogleAuthService _googleAuthService;
    private UtilsFacade _utilsFacade;

    public QuartosFacade(Context context, GoogleAuthService googleAuthService, UtilsFacade utilsFacade)
    {
      _context = context;
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
    }

    // Implementation of IRetorno properties  
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }

    public async Task<IRetorno<IEnumerable<QuartosModel>>> GetAllQuartos(string hotelId)
    {
      try
      {
        var hotelGuid = Guid.Parse(hotelId);

        var quartos = await _context.Quartos.Where(u => u.DetalhesModelId == hotelGuid)
                                            .AsNoTracking()
                                            .ToListAsync();

        return Retorno<IEnumerable<QuartosModel>>.Ok(quartos, "Dados dos quartos buscados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<IEnumerable<QuartosModel>>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno<QuartosModel>> GetQuartoById(string quartoId)
    {
      try
      {
        var quartoGuid = Guid.Parse(quartoId);

        var quarto = await _context.Quartos.Where(u => u.Id == quartoGuid)
                                            .Include(q => q.Category)
                                            .Include(q => q.Beds)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync();

        if (quarto == null)
          return Retorno<QuartosModel>.Erro("Não foi encontrado o quarto.");

        return Retorno<QuartosModel>.Ok(quarto, "Dados do quarto buscados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<QuartosModel>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno> PostQuartosFacade(QuartosModel quartos, string hotelId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado - id.");

        var hotelGuid = Guid.Parse(hotelId);

        // Verifica permissão (admin/manager) para criar ou atualizar quartos
        bool userHasPerm = await _utilsFacade.IsAdminOrManager(user.Id, hotelGuid);
        if (!userHasPerm)
          return Retorno.Erro("Permissão do usuário não é admin/gerente deste hotel.");

        // Busca quarto existente (PUT)
        var quartoExistente = await _context.Quartos
                                            .Include(q => q.Category)
                                            .Include(q => q.Beds)
                                            .FirstOrDefaultAsync(q => q.Id == quartos.Id);

        List<CategoryQuarto> resolvedCats = new List<CategoryQuarto>();

        if (quartos.Category.Any())
        {
          resolvedCats = await _context.CategoryQuarto
                                          .Where(c => quartos.Category.Select(rc => rc.Id).Contains(c.Id))
                                          .ToListAsync();
        }
        

        if (quartoExistente != null)
        { 
          quartoExistente.Name = quartos.Name;
          quartoExistente.Tags = quartos.Tags;
          quartoExistente.Description = quartos.Description;
          quartoExistente.MaxOcupation = quartos.MaxOcupation;
          quartoExistente.Refund = quartos.Refund;
          quartoExistente.AreaSize = quartos.AreaSize;
          quartoExistente.Beds = quartos.Beds;
          quartoExistente.Diff = quartos.Diff;
          quartoExistente.Freeze = quartos.Freeze;
          quartoExistente.Vault = quartos.Vault;
          quartoExistente.Telephone = quartos.Telephone;
          quartoExistente.Coffee = quartos.Coffee;
          quartoExistente.Wifi = quartos.Wifi;
          quartoExistente.Fridge = quartos.Fridge;
          quartoExistente.Cleaning = quartos.Cleaning;
          quartoExistente.Varanda = quartos.Varanda;
          quartoExistente.Bathroom = quartos.Bathroom;
          quartoExistente.BathProducts = quartos.BathProducts;
          quartoExistente.Tv = quartos.Tv;
          quartoExistente.TypeTv = quartos.TypeTv;


          if (resolvedCats.Any())
            quartoExistente.Category = resolvedCats;

          await _context.SaveChangesAsync();
          return Retorno.Ok("Dados do quarto atualizados com sucesso.");
        }
        
        quartos.DetalhesModelId = hotelGuid;

        if (resolvedCats.Any())
          quartos.Category = resolvedCats;

        await _context.Quartos.AddAsync(quartos);
        await _context.SaveChangesAsync();

        return Retorno.Ok("Dados foram registrados com sucesso.");
      }
      catch (InvalidOperationException ioe)
      {
        return Retorno.Erro(ioe.Message);
      }
      catch (KeyNotFoundException knf)
      {
        return Retorno.Erro(knf.Message);
      }
      catch (Exception e)
      {
        return Retorno.Excecao(e, "Erro ao processar a solicitação.");
      }
    }
  }
}

