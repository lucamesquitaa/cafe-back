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

        var quartos = await _context.Quartos.Where(u => u.HotelId == hotelGuid)
                                            .Include(q => q.Category)
                                            .Include(q => q.Beds)
                                            .OrderBy(o => o.Numero)
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
        quartos.HotelId = hotelGuid;

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

    public async Task<IRetorno<QuartosMassaResultDTO>> PostQuartosMassa(string hotelId, CriarQuartosMassaDTO dto)
    {
      try
      {
        var hotelGuid = Guid.Parse(hotelId);

        // Validar se TipoQuartoId pertence ao hotel
        var tipoQuarto = await _context.CategoryQuarto
            .Include(c => c.ConfiguracaoCamas)
            .FirstOrDefaultAsync(c => c.Id == dto.TipoQuartoId && c.HotelId == hotelGuid && c.DeletedAt == null);

        if (tipoQuarto == null)
          return Retorno<QuartosMassaResultDTO>.Erro("Tipo de quarto não encontrado ou não pertence ao hotel.");

        // Montar lista de números a criar
        List<int> numeros;
        if (dto.ModoGeracao == "lista")
        {
          if (dto.ListaManual == null || dto.ListaManual.Count == 0)
            return Retorno<QuartosMassaResultDTO>.Erro("ListaManual é obrigatória no modo 'lista'.");
          numeros = dto.ListaManual.Distinct().OrderBy(n => n).ToList();
        }
        else // range
        {
          if (dto.RangeInicio == null || dto.RangeFim == null)
            return Retorno<QuartosMassaResultDTO>.Erro("RangeInicio e RangeFim são obrigatórios no modo 'range'.");
          if (dto.RangeInicio >= dto.RangeFim)
            return Retorno<QuartosMassaResultDTO>.Erro("RangeInicio deve ser menor que RangeFim.");
          var quantidade = dto.RangeFim.Value - dto.RangeInicio.Value + 1;
          if (quantidade > 200)
            return Retorno<QuartosMassaResultDTO>.Erro("O range não pode superar 200 quartos por chamada.");
          numeros = Enumerable.Range(dto.RangeInicio.Value, quantidade).ToList();
        }

        // Verificar conflitos com quartos existentes no hotel
        var numerosExistentes = await _context.Quartos
            .Where(q => q.HotelId == hotelGuid && numeros.Contains(q.Numero) && q.DeletedAt == null)
            .Select(q => q.Numero)
            .ToListAsync();

        if (numerosExistentes.Any())
          return Retorno<QuartosMassaResultDTO>.Erro(
              "Conflito: já existem quartos com os números informados. Nenhum quarto foi criado.",
              new QuartosMassaResultDTO { Criados = 0, Numeros = new List<int>(), Conflitos = numerosExistentes });

        // Gerar os quartos herdando dados do tipo
        var novosQuartos = numeros.Select(numero => new QuartosModel
        {
          Id = Guid.NewGuid(),
          HotelId = hotelGuid,
          Numero = numero,
          Name = $"{tipoQuarto.Name} {numero}",
          Description = tipoQuarto.Descricao ?? string.Empty,
          MaxOcupation = tipoQuarto.MaxHospedes ?? 0,
          Diff = dto.Andar,
          Category = new List<CategoryQuarto> { tipoQuarto },
          Beds = tipoQuarto.ConfiguracaoCamas
              .Select(b => new BedsDTO { BedType = b.BedType, Quantity = b.Quantity })
              .ToList(),
        }).ToList();

        await _context.Quartos.AddRangeAsync(novosQuartos);
        await _context.SaveChangesAsync();

        var result = new QuartosMassaResultDTO
        {
          Criados = novosQuartos.Count,
          Numeros = numeros,
          Conflitos = new List<int>()
        };

        return Retorno<QuartosMassaResultDTO>.Ok(result);
      }
      catch (Exception e)
      {
        throw new Exception("Erro ao processar a solicitação. hotelId: " + hotelId + " detalhe: " + e.Message);
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
            .Where(r => r.RoomId == quartoGuid && r.Checkout > now && r.CancelledAt == null)
            .ToListAsync();

        foreach (var reserva in reservasFuturas)
        {
          reserva.ReservaStatus = (int)StatusHospedeReservaEnum.CanceladaHotel;
          reserva.CancelledAt = now;
        }

        // Remover tarifas futuras (hard delete — sem valor histórico)
        var tarifasFuturas = await _context.QuartoAvailable
            .Where(a => a.RoomId == quartoGuid && a.EndDate > now)
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

