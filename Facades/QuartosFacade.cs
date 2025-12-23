using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Models.Enums;
using Turify.Services;

namespace Turify.Facades
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
        throw new Exception("Erro ao processar a solicitação. id: " + hotelId);
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
        throw new Exception("Erro ao processar a solicitação. id: " + quartoId);
      }
    }

    public async Task<IRetorno<QuartosModel>> PostQuartosFacade(QuartosModel quartos, string hotelId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno<QuartosModel>.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno<QuartosModel>.Erro("Usuário não encontrado - id.");

        var hotelGuid = Guid.Parse(hotelId);

        bool userHasPerm = await _utilsFacade.IsAdminOrManager(user.Id, hotelGuid);
        if (!userHasPerm)
          return Retorno<QuartosModel>.Erro("Permissão do usuário não é admin/gerente deste hotel.");

        var quartoExistente = await _context.Quartos
                                            .Include(q => q.Category)
                                            .Include(q => q.Beds)
                                            .FirstOrDefaultAsync(q => q.Id == quartos.Id);

        List<CategoryQuarto> resolvedCats = new List<CategoryQuarto>();

        // Resolve categorias (mesma lógica que já está implementada)
        if (quartos.Category != null && quartos.Category.Any())
        {
          var ids = quartos.Category.Select(rc => rc.Id).ToList();
          var categoriesFromDb = await _context.CategoryQuarto.Where(c => ids.Contains(c.Id)).ToListAsync();
          if (categoriesFromDb.Count != ids.Count)
            return Retorno<QuartosModel>.Erro("Uma ou mais categorias informadas não foram encontradas.");
          resolvedCats = categoriesFromDb;
        }

        if (quartoExistente != null)
        {
          // Atualiza campos comuns...
          quartoExistente.Name = quartos.Name;
          quartoExistente.Description = quartos.Description;
          quartoExistente.MaxOcupation = quartos.MaxOcupation;
          quartoExistente.Refund = quartos.Refund;
          quartoExistente.AreaSize = quartos.AreaSize;
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
          quartoExistente.Beds = quartos.Beds;

          if (resolvedCats.Any())
            quartoExistente.Category = resolvedCats;
          else if (quartos.Category != null && !quartos.Category.Any())
            quartoExistente.Category = new List<CategoryQuarto>();

          await _context.SaveChangesAsync();
          return Retorno<QuartosModel>.Ok(quartoExistente);
        }

        // INSERT
        quartos.DetalhesModelId = hotelGuid;

        if (resolvedCats.Any())
          quartos.Category = resolvedCats;

        await _context.Quartos.AddAsync(quartos);
        await _context.SaveChangesAsync();

        return Retorno<QuartosModel>.Ok(quartos);
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + hotelId + "obj: " + JsonSerializer.Serialize(quartos));
      }
    }
    public async Task<IRetorno> DeleteQuartosFacade(string quartoId, string hotelId)
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

        bool userHasPerm = await _utilsFacade.IsAdminOnly(user.Id, hotelGuid);
        if (!userHasPerm)
          return Retorno.Erro("Permissão do usuário não é admin deste hotel.");

        var quartoGuid = Guid.Parse(quartoId);

        var quartoExistente = await _context.Quartos
                                            .FirstOrDefaultAsync(q => q.Id == quartoGuid);

        if (quartoExistente == null)
          return Retorno.Erro("Quarto não encontrado.");

        _context.Quartos.Remove(quartoExistente);
        await _context.SaveChangesAsync();

        return Retorno.Ok("Dados deletados com sucesso.");
      } catch (Exception e) {
        throw new Exception("Erro ao processar a solicitação. id: " + hotelId + " quarto id : " + quartoId);
      }
    }
  }
}

