using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Globalization;
using System.Security.AccessControl;
using System.Text.Json;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Models.Enums;

namespace Turify.Facades
{
  public class MotorDeReservasFacade : IMotorDeReservasFacade
  {
    private readonly Context _context;
    private readonly IDistributedCache _redis;

    public MotorDeReservasFacade(Context context, IDistributedCache redis)
    {
      _context = context;
      _redis = redis;
    }

    // Implementation of IRetorno properties 
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }


    public async Task<IRetorno<QuartoAvailable>> PostDisponibilidadeAsync([FromBody] AddDisponibilidadeDTO disponibilidade, string quartoId)
    {
      try
      {
        Guid quartoGuid = new Guid(quartoId);

        var quarto = await _context.Quartos.FirstOrDefaultAsync(q => q.Id == quartoGuid);

        if (quarto == null || quarto.Id == Guid.Empty)
          return Retorno<QuartoAvailable>.Erro("Quarto não encontrado.");

        bool datasOK = await VerificaDatasOK(disponibilidade, quartoGuid);

        if (!datasOK)
          return Retorno<QuartoAvailable>.Erro("Já existe uma disponibilidade cadastrada para o período informado.");

        QuartoAvailable obj = new QuartoAvailable
        {
          Name = quarto.Name,
          isAvailable = disponibilidade.Status == (int)StatusReservaEnum.Disponivel,
          Status = disponibilidade.Status,
          StartDate = disponibilidade.StartDate,
          EndDate = disponibilidade.EndDate,
          DayPrice = disponibilidade.DayPrice,
          MinDays = disponibilidade.MinDays,
          MaxDays = disponibilidade.MaxDays,
          Reembolsavel = disponibilidade.Reembolsavel,
          RoomId = quarto.Id,
          ReservationId = 1111,
        };
        var cacheKey = $"availability:{quartoId}";
        await _redis.RemoveAsync(cacheKey);
        await _context.QuartoAvailable.AddAsync(obj);
        await _context.SaveChangesAsync();

        return Retorno<QuartoAvailable>.Ok(obj, "Disponibilidade adicionada com sucesso.");

      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<IRetorno<QuartoAvailable>> PutDisponibilidadeDayAsync(UpdateDayDisponibilidadeDTO disponibilidadeDay, string quartoId)
    {
      try
      {
        Guid quartoGuid = new Guid(quartoId);

        // Parsing robusto: aceita "yyyy-MM-dd" e "dd/MM/yyyy" (com fallback para pt-BR).
        if (string.IsNullOrWhiteSpace(disponibilidadeDay?.Day))
          return Retorno<QuartoAvailable>.Erro("Dia inválido.");

        DateTime disponiDayDate;
        var dayStr = disponibilidadeDay.Day.Trim();
        var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };

        // Tenta invariant culture (ISO), depois pt-BR, depois tentativa genérica com pt-BR
        if (!DateTime.TryParseExact(dayStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            && !DateTime.TryParseExact(dayStr, formats, new CultureInfo("pt-BR"), DateTimeStyles.None, out parsed)
            && !DateTime.TryParse(dayStr, new CultureInfo("pt-BR"), DateTimeStyles.AssumeLocal, out parsed))
        {
          return Retorno<QuartoAvailable>.Erro("Formato de data inválido. Use yyyy-MM-dd ou dd/MM/yyyy.");
        }

        disponiDayDate = DateTime.SpecifyKind(parsed.Date, DateTimeKind.Utc);

        // Normaliza o dia para o intervalo [00:00,23:59:59.999...]
        var dayStart = disponiDayDate;
        var dayEnd = disponiDayDate.Date.AddDays(1).AddTicks(-1);

        // Todas as áreas que se sobrepõem ao dia (por dia)
        var areaReserva = await _context.QuartoAvailable
          .Where(d => d.RoomId == quartoGuid && d.EndDate >= dayStart && d.StartDate <= dayEnd)
          .OrderBy(d => d.StartDate)
          .ToListAsync();

        // Se não encontrar nenhuma área, retorna erro
        if (areaReserva == null || areaReserva.Count == 0)
          return Retorno<QuartoAvailable>.Erro("Nenhuma disponibilidade encontrada para o dia informado.");

        foreach (var area in areaReserva)
        {
          // Remove a área original
          _context.QuartoAvailable.Remove(area);

          // Left
          if (area.StartDate < dayStart)
          {
            var left = new QuartoAvailable
            {
              Name = area.Name,
              isAvailable = area.isAvailable,
              Status = area.Status,
              StartDate = area.StartDate,
              EndDate = dayStart.AddTicks(-1),
              DayPrice = area.DayPrice,
              MinDays = area.MinDays,
              MaxDays = area.MaxDays,
              Reembolsavel = area.Reembolsavel,
              RoomId = area.RoomId
            };
            await _context.QuartoAvailable.AddAsync(left);
          }

          // Day (atualiza com os novos dados)
          var daySegment = new QuartoAvailable
          {
            Name = area.Name,
            isAvailable = true,
            Status = (int)StatusReservaEnum.Disponivel,
            StartDate = dayStart,
            EndDate = dayEnd,
            DayPrice = disponibilidadeDay.DayPrice,
            MinDays = disponibilidadeDay.MinDays,
            MaxDays = disponibilidadeDay.MaxDays,
            Reembolsavel = true,
            RoomId = area.RoomId
          };
          await _context.QuartoAvailable.AddAsync(daySegment);

          // Right
          if (area.EndDate > dayEnd)
          {
            var right = new QuartoAvailable
            {
              Name = area.Name,
              isAvailable = area.isAvailable,
              Status = area.Status,
              StartDate = dayEnd.AddTicks(1),
              EndDate = area.EndDate,
              DayPrice = area.DayPrice,
              MinDays = area.MinDays,
              MaxDays = area.MaxDays,
              Reembolsavel = area.Reembolsavel,
              RoomId = area.RoomId
            };
            await _context.QuartoAvailable.AddAsync(right);
          }

        }

        var cacheKey = $"availability:{quartoId}";
        await _redis.RemoveAsync(cacheKey);
        await _context.SaveChangesAsync();

        return Retorno<QuartoAvailable>.Ok(null, "Disponibilidade atualizada com sucesso.");

      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<IRetorno<IEnumerable<QuartoAvailable>>> GetDisponibilidadeAsync(string quartoId)
    {
      try
      {
        var cacheKey = $"availability:{quartoId}";

        //1. Tenta buscar no cache
        var cached = await _redis.GetStringAsync(cacheKey);
        if (cached != null)
        {
          return Retorno<IEnumerable<QuartoAvailable>>.Ok(JsonSerializer.Deserialize<IEnumerable<QuartoAvailable>>(cached));
        }

        //2. Busca no banco (fonte de verdade)
        Guid quartoGuid = new Guid(quartoId);
        var disponibilidades = await _context.QuartoAvailable
        .Where(qa => qa.RoomId == quartoGuid)
        .AsNoTracking()
        .ToListAsync();

        //3. Salva no cache com TTL curto
        await _redis.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(disponibilidades),
        new DistributedCacheEntryOptions
        {
          AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        });



        return Retorno<IEnumerable<QuartoAvailable>>.Ok(disponibilidades, "Disponibilidades buscadas com sucesso.");
      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<IRetorno<QuartoReservas>> PostReservaAsync([FromBody] AddReservaDTO reserva, string quartoId)
    {
      try
      {
        Guid quartoGuid = new Guid(quartoId);

        var quarto = await _context.Quartos.FirstOrDefaultAsync(q => q.Id == quartoGuid);

        if (quarto == null || quarto.Id == Guid.Empty)
          return Retorno<QuartoReservas>.Erro("Quarto não encontrado.");

        QuartoReservas obj = new QuartoReservas
        {
          RoomId = quarto.Id,
          ReservaStatus = (int)reserva.ReservaStatus,
          Checkin = reserva.Checkin,
          Checkout = reserva.Checkout,
          EarlyCheckin = reserva.EarlyCheckin ?? false,
          LateCheckout = reserva.LateCheckout ?? false,
          Adults = reserva.Adults,
          Kids = reserva.Kids,
          Cupom = reserva.Cupom,
          PriceTotal = reserva.PriceTotal,
          CreatedAt = DateTime.UtcNow,
        };

        // Todas as áreas que se sobrepõem ao período (por tempo)
        var areaReserva = await _context.QuartoAvailable
          .Where(d => d.RoomId == quartoGuid && d.EndDate >= reserva.Checkin && d.StartDate <= reserva.Checkout)
          .OrderBy(d => d.StartDate)
          .ToListAsync();

        if (areaReserva == null || areaReserva.Count ==0)
          return Retorno<QuartoReservas>.Erro("Verifique se existe cadastro para o período informado.");

        // Não permitir EarlyCheckin se já existir qualquer checkout na mesma data
        if (reserva.EarlyCheckin == true)
        {
          var hasAnyCheckoutOnCheckinDate = await _context.QuartoReservas
            .AnyAsync(r => r.RoomId == quartoGuid && r.Checkout.Date == reserva.Checkin.Date);

          if (hasAnyCheckoutOnCheckinDate)
            return Retorno<QuartoReservas>.Erro("Early check-in não permitido: já existe checkout na mesma data.");
        }

        // Tratamento especial: permitir que o checkin coincida com o checkout de outra reserva
        // desde que não exista uma reserva com LateCheckout = true para essa data.
        bool hasLateCheckoutOnCheckinDate = await _context.QuartoReservas
          .AnyAsync(r => r.RoomId == quartoGuid && r.Checkout.Date == reserva.Checkin.Date && (r.LateCheckout ?? false));

        if (hasLateCheckoutOnCheckinDate)
        {
          // Se houver late checkout na data de checkin, qualquer segmento indisponível bloqueia.
          if (areaReserva.Any(a => !a.isAvailable))
            return Retorno<QuartoReservas>.Erro("Verifique a disponibilidade para o período informado.");
        }
        else
        {
          // Se não houver late checkout, podemos ignorar colisões que sejam apenas o dia de checkin
          var nonAvailable = areaReserva.Where(a => !a.isAvailable).ToList();
          bool blocked = false;
          foreach (var area in nonAvailable)
          {
            // Calcula interseção por dias (mesma lógica usada ao criar segmentos)
            var nightStart = reserva.Checkin.Date;
            var nightEnd = reserva.Checkout.Date;

            var overlapNightStart = area.StartDate.Date > nightStart ? area.StartDate.Date : nightStart;
            var overlapNightEnd = area.EndDate.Date < nightEnd ? area.EndDate.Date : nightEnd;

            bool hasTimeOverlap = area.EndDate >= reserva.Checkin && area.StartDate <= reserva.Checkout;
            if (overlapNightStart > overlapNightEnd && hasTimeOverlap)
            {
              overlapNightStart = reserva.Checkin.Date;
              overlapNightEnd = overlapNightStart;
            }

            // Se a interseção for apenas a noite de checkin (um único dia igual ao checkin), ignoramos esse conflito.
            if (!(overlapNightStart == overlapNightEnd && overlapNightStart == reserva.Checkin.Date))
            {
              blocked = true;
              break;
            }
          }

          if (blocked)
            return Retorno<QuartoReservas>.Erro("Verifique a disponibilidade para o período informado.");
        }

        if(areaReserva.Select(areaReserva => areaReserva.MinDays).Any(minDays => (reserva.Checkout.Date - reserva.Checkin.Date).TotalDays < minDays))
          return Retorno<QuartoReservas>.Erro("O período reservado é menor que o mínimo permitido para o período.");

        if((areaReserva.Select(areaReserva => areaReserva.MaxDays).Any(max => max != 0 && (reserva.Checkout.Date - reserva.Checkin.Date).TotalDays > max)))
          return Retorno<QuartoReservas>.Erro("O período reservado é maior que o máximo permitido para o período.");

        foreach (var area in areaReserva)
        {
          // Remove a área original
          _context.QuartoAvailable.Remove(area);

          // Agora tratamos por dias e incluímos a data de checkout como dia reservado.
          // Se EarlyCheckin == true, bloqueia o dia anterior ao checkin também
          var nightStart = reserva.EarlyCheckin == true
              ? reserva.Checkin.Date.AddDays(-1)
              : reserva.Checkin.Date; // Sem early checkin, começa no dia do checkin

          // Se LateCheckout == true, bloqueia o dia completo do checkout também
          var nightEnd = reserva.LateCheckout == true
              ? reserva.Checkout.Date
              : reserva.Checkout.Date.AddDays(-1); // Sem late checkout, não bloq late checkout, não bloqueia o último dia

          // Interseção em dias entre a área e a reserva
          var overlapNightStart = area.StartDate.Date > nightStart ? area.StartDate.Date : nightStart;
          var overlapNightEnd = area.EndDate.Date < nightEnd ? area.EndDate.Date : nightEnd;

          // Se não houve interseção por dias mas houve por horário, marque ao menos a noite de checkin
          bool hasTimeOverlap = area.EndDate >= reserva.Checkin && area.StartDate <= reserva.Checkout;
          if (overlapNightStart > overlapNightEnd && hasTimeOverlap)
          {
            overlapNightStart = reserva.Checkin.Date;
            overlapNightEnd = overlapNightStart;
          }

          if (overlapNightStart > overlapNightEnd)
            continue;

          // Boundaries em DateTime para banco (bookedStart = 00:00 do dia; bookedEnd = fim do dia)
          var bookedStart = overlapNightStart;
          var bookedEnd = overlapNightEnd.AddDays(1).AddTicks(-1);

          // Left
          if (area.StartDate < bookedStart)
          {
            var left = new QuartoAvailable
            {
              Name = area.Name,
              isAvailable = area.isAvailable,
              Status = area.Status,
              StartDate = area.StartDate,
              EndDate = bookedStart.AddTicks(-1),
              DayPrice = area.DayPrice,
              MinDays = area.MinDays,
              MaxDays = area.MaxDays,
              Reembolsavel = area.Reembolsavel,
              RoomId = area.RoomId
            };
            await _context.QuartoAvailable.AddAsync(left);
          }

          // Booked (indisponível)
          var bookedSegment = new QuartoAvailable
          {
            Name = area.Name,
            isAvailable = false,
            Status = (int)StatusReservaEnum.AguardandoPagamento,
            StartDate = bookedStart,
            EndDate = bookedEnd,
            DayPrice = area.DayPrice,
            MinDays = area.MinDays,
            MaxDays = area.MaxDays,
            Reembolsavel = area.Reembolsavel,
            RoomId = area.RoomId
          };
          await _context.QuartoAvailable.AddAsync(bookedSegment);

          // Right
          if (area.EndDate > bookedEnd)
          {
            var right = new QuartoAvailable
            {
              Name = area.Name,
              isAvailable = area.isAvailable,
              Status = area.Status,
              StartDate = bookedEnd.AddTicks(1),
              EndDate = area.EndDate,
              DayPrice = area.DayPrice,
              MinDays = area.MinDays,
              MaxDays = area.MaxDays,
              Reembolsavel = area.Reembolsavel,
              RoomId = area.RoomId
            };
            await _context.QuartoAvailable.AddAsync(right);
          }
        }
        var cacheKey = $"availability:{quartoId}";
        await _redis.RemoveAsync(cacheKey);
        await _context.QuartoReservas.AddAsync(obj);
        await _context.SaveChangesAsync();

        return Retorno<QuartoReservas>.Ok(obj, "Reserva criada com sucesso.");
      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<IRetorno> PutReservaAsync(UpdateReservaDTO updatedReserva, string quartoId)
    {
      try
      {
        Guid reservaGuid = new Guid(updatedReserva.ReservaId);
        var reserva = await _context.QuartoReservas.FirstOrDefaultAsync(r => r.Id == reservaGuid);

        if (reserva == null || reserva.Id == Guid.Empty)
          return Retorno<IEnumerable<QuartoReservas>>.Erro("Reserva não encontrada.");

        reserva.ReservaStatus = (int)updatedReserva.ReservaStatus;
        reserva.Adults = updatedReserva.Adults;
        reserva.Kids = updatedReserva.Kids;
        reserva.PriceTotal = updatedReserva.PriceTotal;
        // 1. Remover hóspedes existentes e salvar imediatamente
        var hospedesExistentes = await _context.Hospedes
            .Where(h => h.ReservationId == reservaGuid)
            .ToListAsync();

        if (hospedesExistentes.Any())
        {
          _context.Hospedes.RemoveRange(hospedesExistentes);
          await _context.SaveChangesAsync(); // IMPORTANTE: Salvar antes de adicionar novos
        }

        // 2. Adicionar os novos hóspedes
        var novosHospedes = updatedReserva.Hospede.Select(h => new Hospedes
        {
          ReservationId = reservaGuid,
          Name = h.Name,
          FamilyName = h.FamilyName,
          Email = h.Email,
          CPF = h.CPF,
          Phone = h.Phone,
          DateBirth = h.DateBirth,
          CEP = h.CEP,
          State = h.State,
          City = h.City,
          Address = h.Address,
          Complement = h.Complement,
          BloodType = h.BloodType,
          Principal = h.Principal,
          TextArea = h.TextArea
        }).ToList();

        await _context.Hospedes.AddRangeAsync(novosHospedes);

        await _context.SaveChangesAsync();
        return Retorno.Ok(null, "Reserva atualizada com sucesso.");
      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<IRetorno<IEnumerable<RetornoReservasDTO>>> GetReservaAsync(string quartoId)
    {
      try
      {
        Guid quartoGuid = new Guid(quartoId);

        var reservas = await _context.QuartoReservas
        .Where(qa => qa.RoomId == quartoGuid)
        .Include(h => h.Hospede)
        .Select(r => new RetornoReservasDTO
        {
          Id = r.Id.ToString(),
          RoomId = r.RoomId.ToString(),
          ReservaStatus = r.ReservaStatus,
          Checkin = r.Checkin,
          Checkout = r.Checkout,
          EarlyCheckin = r.EarlyCheckin,
          LateCheckout = r.LateCheckout,
          Adults = r.Adults,
          Kids = r.Kids,
          Cupom = r.Cupom,
          PriceTotal = r.PriceTotal,
          CreatedAt = r.CreatedAt,
          Hospede = r.Hospede.Select(h => new HospedeDTO
          {
            Name = h.Name,
            FamilyName = h.FamilyName,
            Email = h.Email,
            CPF = h.CPF,
            Phone = h.Phone,
            DateBirth = h.DateBirth,
            CEP = h.CEP,
            State = h.State,
            City = h.City,
            Address = h.Address,
            Complement = h.Complement,
            BloodType = h.BloodType,
            Principal = h.Principal,
            TextArea = h.TextArea
          }).ToList()
        })
        .AsNoTracking()
        .ToListAsync();

        return Retorno<IEnumerable<RetornoReservasDTO>>.Ok(reservas, "reservas buscadas com sucesso.");
      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<IRetorno> DeleteReservaAsync(string reservaId, string quartoId)
    {
      try
      {
        Guid reservaGuid = new Guid(reservaId);
        var reserva = await _context.QuartoReservas.FirstOrDefaultAsync(r => r.Id == reservaGuid);

        if (reserva == null || reserva.Id == Guid.Empty)
          return Retorno<QuartoReservas>.Erro("Reserva não encontrada.");

        reserva.ReservaStatus = (int)StatusHospedeReservaEnum.CanceladaHotel;
        reserva.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Retorno.Ok(null, "Reserva cancelada com sucesso.");
      }
      catch (Exception e)
      {
        throw new ArgumentException(e.Message);
      }
    }

    public async Task<bool> VerificaDatasOK(AddDisponibilidadeDTO disponibilidade, Guid quartoId)
    {
      var disponiExistente = await _context.QuartoAvailable.Where(d =>
      d.RoomId == quartoId &&
      d.StartDate <= disponibilidade.EndDate &&
      d.EndDate >= disponibilidade.StartDate
      ).ToListAsync();
      if (disponiExistente != null && disponiExistente.Count > 0)
        return false;
      else
        return true;
    }

    public async Task<bool> VerificaDatasReserva(AddReservaDTO reserva, Guid quartoId)
    {
      // Cobertura por dias; incluir a data de checkout
      var overlapping = await _context.QuartoAvailable
        .Where(d => d.RoomId == quartoId && d.EndDate >= reserva.Checkin && d.StartDate <= reserva.Checkout)
        .OrderBy(d => d.StartDate)
        .ToListAsync();

      if (overlapping == null || overlapping.Count == 0)
        return false;

      DateTime current = reserva.Checkin.Date;
      DateTime lastNight = reserva.Checkout.Date; // <-- incluir checkout

      foreach (var seg in overlapping)
      {
        if (!seg.isAvailable)
          return false;

        if (seg.StartDate.Date > current)
          return false;

        if (seg.EndDate.Date >= current)
        {
          current = seg.EndDate.Date.AddDays(1);
        }

        if (current > lastNight)
          return true;
      }

      return current > lastNight;
    }
  }
}