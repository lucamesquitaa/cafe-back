using Microsoft.EntityFrameworkCore;
using Cafeteria.Data;
using Cafeteria.Facades.Interfaces;
using Cafeteria.Models;
using Cafeteria.Models.DTOs;
using Cafeteria.Models.Enums;
using Cafeteria.Services;

namespace Cafeteria.Facades
{
  public class CafeteriaFacade : ICafeteriaFacade, IRetorno
  {
    private readonly Context _context;
    private readonly GoogleAuthService _googleAuthService;
    private readonly UtilsFacade _utilsFacade;
    private readonly PhotosFacade _photosFacade;

    public CafeteriaFacade(Context context, GoogleAuthService googleAuthService, UtilsFacade utilsFacade, PhotosFacade photosFacade)
    {
      _context = context;
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
      _photosFacade = photosFacade;
    }

    // Implementation of IRetorno properties
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }
    public object? Data { get; private set; }

    public async Task<IRetorno<IEnumerable<GetAllCafeterias>>> GetAllFacade(int page, int pageSize)
    {
      try
      {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var cafeterias = await _context.Cafeterias.AsNoTracking().ToListAsync();

        var resultado = cafeterias
          .OrderByDescending(c => c.CriadoEm)
          .Select(c => new GetAllCafeterias
          {
            Id = c.Id,
            Nome = c.Nome,
            Endereco = c.Endereco,
            Numero = c.Numero,
            Complemento = c.Complemento,
            Cep = c.Cep,
            FotoPrincipal = c.FotoPrincipal,
            CategoriaPrincipal = c.CategoriaPrincipal
          });

        var paginado = resultado.Skip((page - 1) * pageSize).Take(pageSize);

        return Retorno<IEnumerable<GetAllCafeterias>>.Ok(paginado, "Cafeterias buscadas com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação.");
      }
    }
    public async Task<IRetorno<CafeteriaModel>> GetByManagerFacade(string id)
    {
      try
      {
        if (!Guid.TryParse(id, out var cafeteriaId))
          return Retorno<CafeteriaModel>.Erro("Id inválido.");

        var cafeteria = await _context.Cafeterias.AsNoTracking()
         .Where(c => c.Id == cafeteriaId)
         .FirstOrDefaultAsync();


        if (cafeteria == null)
          return Retorno<CafeteriaModel>.Erro("Cafeteria não encontrada.");

        return Retorno<CafeteriaModel>.Ok(cafeteria, "Cafeteria encontrada com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + id);
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
            Rede = c.Rede,
            Url = c.Url,
            Descricao = c.Descricao,
            Diferencial = c.Diferencial,
            Ativo = c.Ativo,
            Endereco = c.Endereco,
            Numero = c.Numero,
            Cep = c.Cep,
            Cidade = c.Cidade,
            Estado = c.Estado,
            Complemento = c.Complemento,
            FotoPrincipal = c.FotoPrincipal,
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

        string? fotoPrincipalUrl = null;

        if (cafeteria.FotoPrincipal != null && cafeteria.FotoPrincipal.Length > 0)
        {
          var uploadRetorno = await _photosFacade.UploadFotoPrincipalAsync(cafeteria.FotoPrincipal, cafeteria.Nome);

          if (!uploadRetorno.Sucesso)
            return Retorno<GetCafeteriaById>.Erro(uploadRetorno.Mensagem ?? "Erro ao enviar a foto principal.");

          fotoPrincipalUrl = uploadRetorno.Data;
        }

        var novaCafeteria = new Models.CafeteriaModel
        {
          Id = novoId,
          Nome = cafeteria.Nome,
          Rede = cafeteria.Rede,
          Url = cafeteria.Url,
          Descricao = cafeteria.Descricao,
          Diferencial = cafeteria.Diferencial,
          Endereco = cafeteria.Endereco,
          Numero = cafeteria.Numero,
          Cep = cafeteria.Cep,
          Complemento = cafeteria.Complemento,
          Cidade = "Belo Horizonte",
          Estado = "MG",
          FotoPrincipal = fotoPrincipalUrl,
          NomeRep = cafeteria.NomeRep,
          TelRep = cafeteria.TelRep,
          CpfRep = cafeteria.CpfRep,
          EmailRep = cafeteria.EmailRep,
          Cnpj = cafeteria.Cnpj,
          Razao = cafeteria.Razao,
          CategoriaPrincipal = TypeCafeEnum.Cafeteria,
          CriadoEm = DateTime.UtcNow,
          Ativo = true
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
          Rede = novaCafeteria.Rede,
          Url = novaCafeteria.Url,
          Descricao = novaCafeteria.Descricao,
          Diferencial = novaCafeteria.Diferencial,
          Ativo = novaCafeteria.Ativo,
          Endereco = novaCafeteria.Endereco,
          Numero = novaCafeteria.Numero,
          Cep = novaCafeteria.Cep,
          Cidade = novaCafeteria.Cidade,
          Estado = novaCafeteria.Estado,
          Complemento = novaCafeteria.Complemento,
          FotoPrincipal = novaCafeteria.FotoPrincipal,
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

        string? fotoPrincipalUrl = existente.FotoPrincipal;

        if (cafeteria.FotoPrincipal != null && cafeteria.FotoPrincipal.Length > 0)
        {
          var uploadRetorno = await _photosFacade.UploadFotoPrincipalAsync(cafeteria.FotoPrincipal, cafeteria.Nome);

          if (!uploadRetorno.Sucesso)
            return Retorno<GetCafeteriaById>.Erro(uploadRetorno.Mensagem ?? "Erro ao enviar a foto principal.");

          fotoPrincipalUrl = uploadRetorno.Data;
        }

        existente.Nome = cafeteria.Nome;
        existente.Rede = cafeteria.Rede;
        existente.Url = cafeteria.Url;
        existente.Descricao = cafeteria.Descricao;
        existente.Diferencial = cafeteria.Diferencial;
        existente.Ativo = cafeteria.Ativo;
        existente.Endereco = cafeteria.Endereco;
        existente.Numero = cafeteria.Numero;
        existente.Cep = cafeteria.Cep;
        existente.Cidade = cafeteria.Cidade;
        existente.Estado = cafeteria.Estado;
        existente.Complemento = cafeteria.Complemento;
        existente.FotoPrincipal = fotoPrincipalUrl;
        existente.CategoriaPrincipal = cafeteria.CategoriaPrincipal;

        await _context.SaveChangesAsync();

        return Retorno<GetCafeteriaById>.Ok(new GetCafeteriaById
        {
          Id = existente.Id,
          Nome = existente.Nome,
          Rede = existente.Rede,
          Url = existente.Url,
          Descricao = existente.Descricao,
          Diferencial = existente.Diferencial,
          Ativo = existente.Ativo,
          Endereco = existente.Endereco,
          Numero = existente.Numero,
          Cep = existente.Cep,
          Cidade = existente.Cidade,
          Estado = existente.Estado,
          Complemento = existente.Complemento,
          FotoPrincipal = existente.FotoPrincipal,
          CategoriaPrincipal = existente.CategoriaPrincipal,
          CriadoEm = existente.CriadoEm
        }, "Cafeteria atualizada com sucesso.");
      }
      catch (Exception)
      {
        throw new Exception("Erro ao processar a solicitação. id: " + id);
      }
    }
  }
}
