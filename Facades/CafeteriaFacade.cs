using Microsoft.EntityFrameworkCore;
using Turify.Data;
using Turify.Facades.Interfaces;
using Turify.Models;
using Turify.Models.DTOs;
using Turify.Models.Enums;
using Turify.Services;

namespace Turify.Facades
{
  public class CafeteriaFacade : ICafeteriaFacade, IRetorno
  {
    private readonly Context _context;
    private readonly GoogleAuthService _googleAuthService;
    private readonly UtilsFacade _utilsFacade;

    public CafeteriaFacade(Context context, GoogleAuthService googleAuthService, UtilsFacade utilsFacade)
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

    public async Task<IRetorno<IEnumerable<GetAllCafeterias>>> GetAllFacade(double? lat, double? lng, int page, int pageSize)
    {
      try
      {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var cafeterias = await _context.Cafeterias.AsNoTracking().ToListAsync();

        IEnumerable<GetAllCafeterias> resultado;

        if (lat.HasValue && lng.HasValue)
        {
          resultado = cafeterias
            .Select(c => new GetAllCafeterias
            {
              Id = c.Id,
              Nome = c.Nome,
              Endereco = c.Endereco,
              FotoUrl = c.FotoUrl,
              CategoriaPrincipal = c.CategoriaPrincipal,
              NotaMedia = c.NotaMedia,
              QtdAvaliacoes = c.QtdAvaliacoes,
              DistanciaKm = CalcularDistanciaKm(lat.Value, lng.Value, c.Lat, c.Lng)
            })
            .OrderBy(c => c.DistanciaKm);
        }
        else
        {
          resultado = cafeterias
            .OrderByDescending(c => c.CriadoEm)
            .Select(c => new GetAllCafeterias
            {
              Id = c.Id,
              Nome = c.Nome,
              Endereco = c.Endereco,
              FotoUrl = c.FotoUrl,
              CategoriaPrincipal = c.CategoriaPrincipal,
              NotaMedia = c.NotaMedia,
              QtdAvaliacoes = c.QtdAvaliacoes,
              DistanciaKm = null
            });
        }

        var paginado = resultado.Skip((page - 1) * pageSize).Take(pageSize);

        return Retorno<IEnumerable<GetAllCafeterias>>.Ok(paginado, "Cafeterias buscadas com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno<GetCafeteriaById>> GetByIdFacade(string id)
    {
      try
      {
        if (!Guid.TryParse(id, out var cafeteriaId))
          return Retorno<GetCafeteriaById>.Erro("Id inválido.");

        var cafeteria = await _context.Cafeterias.AsNoTracking()
          .Where(c => c.Id == cafeteriaId)
          .Select(c => new GetCafeteriaById
          {
            Id = c.Id,
            Nome = c.Nome,
            Endereco = c.Endereco,
            Lat = c.Lat,
            Lng = c.Lng,
            NotaMedia = c.NotaMedia,
            QtdAvaliacoes = c.QtdAvaliacoes,
            FotoUrl = c.FotoUrl,
            CategoriaPrincipal = c.CategoriaPrincipal,
            CriadoEm = c.CriadoEm
          })
          .FirstOrDefaultAsync();

        if (cafeteria == null)
          return Retorno<GetCafeteriaById>.Erro("Cafeteria não encontrada.");

        return Retorno<GetCafeteriaById>.Ok(cafeteria, "Cafeteria encontrada com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + id);
      }
    }

    public async Task<IRetorno<GetCafeteriaById>> PostFacade(CriarCafeteriaDTO cafeteria)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno<GetCafeteriaById>.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno<GetCafeteriaById>.Erro("Usuário não encontrado - id.");

        var novoId = Guid.NewGuid();

        var novaCafeteria = new Cafeteria
        {
          Id = novoId,
          Nome = cafeteria.Nome,
          Endereco = cafeteria.Endereco,
          Lat = cafeteria.Lat,
          Lng = cafeteria.Lng,
          CategoriaPrincipal = cafeteria.CategoriaPrincipal,
          FotoUrl = cafeteria.FotoUrl,
        };

        var permissao = new UsuarioPermissoes
        {
          Id = Guid.NewGuid(),
          CafeteriaId = novoId,
          UserModelId = user.Id,
          UserModelEmail = user.Email,
          Role = RoleUserModel.Admin
        };

        await _context.Cafeterias.AddAsync(novaCafeteria);
        await _context.UsuarioPermissao.AddAsync(permissao);
        await _context.SaveChangesAsync();

        return Retorno<GetCafeteriaById>.Ok(new GetCafeteriaById
        {
          Id = novaCafeteria.Id,
          Nome = novaCafeteria.Nome,
          Endereco = novaCafeteria.Endereco,
          Lat = novaCafeteria.Lat,
          Lng = novaCafeteria.Lng,
          NotaMedia = novaCafeteria.NotaMedia,
          QtdAvaliacoes = novaCafeteria.QtdAvaliacoes,
          FotoUrl = novaCafeteria.FotoUrl,
          CategoriaPrincipal = novaCafeteria.CategoriaPrincipal,
          CriadoEm = novaCafeteria.CriadoEm
        }, "Cafeteria criada com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno<GetCafeteriaById>> PutFacade(AtualizarCafeteriaDTO cafeteria, string id)
    {
      try
      {
        if (!Guid.TryParse(id, out var cafeteriaId))
          return Retorno<GetCafeteriaById>.Erro("Id inválido.");

        var existente = await _context.Cafeterias.FirstOrDefaultAsync(c => c.Id == cafeteriaId);

        if (existente == null)
          return Retorno<GetCafeteriaById>.Erro("Cafeteria não encontrada.");

        existente.Nome = cafeteria.Nome;
        existente.Endereco = cafeteria.Endereco;
        existente.CategoriaPrincipal = cafeteria.CategoriaPrincipal;
        existente.FotoUrl = cafeteria.FotoUrl;

        await _context.SaveChangesAsync();

        return Retorno<GetCafeteriaById>.Ok(new GetCafeteriaById
        {
          Id = existente.Id,
          Nome = existente.Nome,
          Endereco = existente.Endereco,
          Lat = existente.Lat,
          Lng = existente.Lng,
          NotaMedia = existente.NotaMedia,
          QtdAvaliacoes = existente.QtdAvaliacoes,
          FotoUrl = existente.FotoUrl,
          CategoriaPrincipal = existente.CategoriaPrincipal,
          CriadoEm = existente.CriadoEm
        }, "Cafeteria atualizada com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + id);
      }
    }

    private static double CalcularDistanciaKm(double lat1, double lng1, double lat2, double lng2)
    {
      const double raioTerraKm = 6371;

      var dLat = DegreesToRadians(lat2 - lat1);
      var dLng = DegreesToRadians(lng2 - lng1);

      var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
              Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

      var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

      return raioTerraKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
  }
}
