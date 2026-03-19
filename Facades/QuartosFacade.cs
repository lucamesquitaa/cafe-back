using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Models.Enums;

namespace Turify.Facades
{
  public class QuartosFacade : IQuartosFacade, IRetorno
  {
    private readonly Context _context;
    private PhotosFacade _photosFacade;

    public QuartosFacade(Context context, PhotosFacade photosFacade)
    {
      _context = context;
      _photosFacade = photosFacade;
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
                                            .Include(q => q.Category)
                                            .Include(q => q.Beds)
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
        var hotelGuid = Guid.Parse(hotelId);

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
          quartoExistente.Numero = quartos.Numero;
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
    public async Task<IRetorno> DeleteQuartosFacade(string hotelId, string quartoId)
    {
      try
      {
        var quartoGuid = Guid.Parse(quartoId);

        var quartoExistente = await _context.Quartos
                                            .FirstOrDefaultAsync(q => q.Id == quartoGuid);

        if (quartoExistente == null)
          return Retorno.Erro("Quarto não encontrado.");

        var now = DateTime.UtcNow;

        // Cancelar reservas futuras (soft delete)
        var reservasFuturas = await _context.QuartoReservas
            .Where(r => r.QuartosModelId == quartoGuid && r.Checkout > now && r.CancelledAt == null)
            .ToListAsync();

        foreach (var reserva in reservasFuturas)
        {
          reserva.ReservaStatus = (int)StatusHospedeReservaEnum.CanceladaHotel;
          reserva.CancelledAt = now;
        }

        // Remover tarifas futuras (hard delete — sem valor histórico)
        var tarifasFuturas = await _context.QuartoAvailable
            .Where(a => a.QuartosModelId == quartoGuid && a.EndDate > now)
            .ToListAsync();

        _context.QuartoAvailable.RemoveRange(tarifasFuturas);

        // Soft delete do quarto
        quartoExistente.DeletedAt = now;

        await _context.SaveChangesAsync();

        return Retorno.Ok("Quarto deletado com sucesso.");
      } catch (Exception e) {
        throw new Exception("Erro ao processar a solicitação. id: " + hotelId + " quarto id : " + quartoId);
      }
    }
  }
}

