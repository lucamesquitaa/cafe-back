using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using RabbitMQ.Stream.Client.Reliable;
using SaudeIA.Data;
using SaudeIA.Facades.Interfaces;
using SaudeIA.Models;
using SaudeIA.Models.DTOs;
using SaudeIA.Models.Enums;
using SaudeIA.Services;

namespace SaudeIA.Facades
{
  public class HotelFacade : IHotelFacade, IRetorno
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

    public HotelFacade(Context context, IRabbitMqProducer producer, GoogleAuthService googleAuthService, UtilsFacade utilsFacade)
    {
      _context = context;
      _producer = producer;
      _googleAuthService = googleAuthService;
      _utilsFacade = utilsFacade;
    }

    public async Task<IRetorno<IEnumerable<GetAllHoteis>>> GetAllFacade()
    {
      try
      {

        var hoteis = await _context.Hotel.AsNoTracking()
           .Select(h => new GetAllHoteis
           {
             Id = h.Id,
             Name = h.Name,
             Description = h.Description,
             Url = h.Url,
           }).ToListAsync();

        return Retorno<IEnumerable<GetAllHoteis>>.Ok(hoteis, "Hoteis buscados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<IEnumerable<GetAllHoteis>>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }
    public async Task<IRetorno<GetDetalheById>> GetDetalhesFacade(string hotelId)
    {
      try
      {
        var hotel = await _context.Hotel.Where(u => u.Id.ToString() == hotelId)
                                        .AsNoTracking()
                                        .Select(u => new GetDetalheById
                                        {
                                          Name = u.Name,
                                          Rede = u.Rede,
                                          City = u.City,
                                          Url = u.Url,
                                          Description = u.Description,
                                          Category = u.Category,
                                          Child = u.Child,
                                          Pets = u.Pets,
                                          PetsTax = u.PetsTax,
                                          Cep = u.Cep,
                                          Address = u.Address,
                                          Number = u.Number,
                                          Complement = u.Complement,
                                          Lobby = u.Lobby,
                                          Diff = u.Diff,
                                          Beach = u.Beach,
                                          Downtown = u.Downtown,
                                          Airpot = u.Airpot,
                                          Highway = u.Highway,
                                          Hospital = u.Hospital,
                                          Coffee = u.Coffee,
                                          Wifi = u.Wifi,
                                          Swimming = u.Swimming,
                                          Cleaning = u.Cleaning,
                                          Gym = u.Gym,
                                        })
                                        .FirstOrDefaultAsync();

        return Retorno<GetDetalheById>.Ok(hotel, "Dados do hotel buscados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<GetDetalheById>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno<DetalhesModel>> GetDetalhesFacadeByManager(string hotelId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno<DetalhesModel>.Erro("Usuario não tem permissão para executar esta ação.");

        var user = _utilsFacade.GetUserByEmail(userEmail);

        bool userHasPerm = await _utilsFacade.IsAdminOrManager(user.Id.ToString(), hotelId);

        if (!userHasPerm)
          return Retorno<DetalhesModel>.Erro("O usuário não tem permissão para executar esta ação.");

        var hotel = await _context.Hotel.Where(u => u.Id.ToString() == hotelId)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();

        return Retorno<DetalhesModel>.Ok(hotel, "Hoteis foram buscados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno<DetalhesModel>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno<IEnumerable<GetAllHoteis>>> GetDetalhesUserFacade()
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno<IEnumerable<GetAllHoteis>>.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user.Id == Guid.Empty)
          return Retorno<IEnumerable<GetAllHoteis>>.Erro("Usuário não encontrado - id.");

        var hotelIds = await _context.UsuarioPermissao
                                  .Where(up => up.UserModelId == user.Id &&
                                                up.Role == RoleUserModel.Admin || up.Role == RoleUserModel.Manager || up.Role == RoleUserModel.Turify)
                                  .Select(up => up.DetalhesModelId)
                                  .ToListAsync();

        var hoteis = await _context.Hotel.Where(u => hotelIds.Contains(u.Id))
                                        .AsNoTracking().Select(h => new GetAllHoteis
                                        {
                                          Id = h.Id,
                                          Name = h.Name,
                                          Description = h.Description,
                                          Url = h.Url,
                                        }).ToListAsync();

        return Retorno<IEnumerable<GetAllHoteis>>.Ok(hoteis, "Dados buscados com sucesso!"); ;
      }
      catch (Exception e)
      {
        return Retorno<IEnumerable<GetAllHoteis>>.Excecao(e, "Erro ao processar a solicitação.");
      }
    }

    public async Task<IRetorno> PostDetalhesFacade(DetalhesModel hotel)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado - id.");

        var urlHotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Url == hotel.Url);
        if (urlHotel != null)
          return Retorno.Erro("Já existe um hotel cadastrado com esta URL.");

        var novoId = Guid.NewGuid();

        var permissao = new UsuarioPermissoes
        {
          Id = Guid.NewGuid(),
          DetalhesModelId = novoId,
          UserModelEmail = userEmail,
          UserModelId = user.Id,
          Role = RoleUserModel.Admin
        };

        var hotelNew = new DetalhesModel
        {
          Id = novoId,
          Name = hotel.Name,
          Rede = hotel.Rede,
          City = hotel.City,
          Url = hotel.Url,
          Description = hotel.Description,
          Category = hotel.Category,
          Child = hotel.Child,
          Pets = hotel.Pets,
          PetsTax = hotel.PetsTax ?? 0,
          Cep = hotel.Cep,
          Address = hotel.Address,
          Number = hotel.Number,
          Complement = hotel.Complement,
          Lobby = hotel.Lobby,
          Diff = hotel.Diff,
          Beach = hotel.Beach ?? false,
          Downtown = hotel.Downtown ?? false,
          Airpot = hotel.Airpot ?? false,
          Highway = hotel.Highway ?? false,
          Hospital = hotel.Hospital ?? false,
          Coffee = hotel.Coffee ?? false,
          Wifi = hotel.Wifi ?? false,
          Swimming = hotel.Swimming ?? false,
          Cleaning = hotel.Cleaning ?? false,
          Gym = hotel.Gym ?? false,
        };

        await _context.Hotel.AddAsync(hotelNew);
        await _context.UsuarioPermissao.AddAsync(permissao);

        await _context.SaveChangesAsync();

        return Retorno.Ok("Dados foram registrados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno.Excecao(e, "Erro ao processar a solicitação.");
      }
    }


    public async Task<IRetorno> PutDetalhesFacade(DetalhesModel hotel, string id)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado - id.");

        bool userHasPerm = await _utilsFacade.IsAdminOrManager(user.Id.ToString(), userEmail);

        if (!userHasPerm)
          return Retorno.Erro("Permissão do usuário não é admin/gerente deste hotel.");

        var hotelId = Guid.Parse(id);

        var hotelExistente = await _context.Hotel
            .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotelExistente == null)
          return Retorno.Erro("Hotel não encontrado.");

        // Atualiza propriedades simples
        hotelExistente.Name = hotel.Name;
        hotelExistente.Rede = hotel.Rede;
        hotelExistente.City = hotel.City;
        hotelExistente.Url = hotel.Url;
        hotelExistente.Description = hotel.Description;
        hotelExistente.Category = hotel.Category;
        hotelExistente.Child = hotel.Child ?? false;
        hotelExistente.Pets = hotel.Pets ?? false;
        hotelExistente.PetsTax = hotel.PetsTax;
        hotelExistente.Cep = hotel.Cep;
        hotelExistente.Address = hotel.Address;
        hotelExistente.Number = hotel.Number;
        hotelExistente.Complement = hotel.Complement;
        hotelExistente.Lobby = hotel.Lobby;
        hotelExistente.Diff = hotel.Diff;
        hotelExistente.Beach = hotel.Beach ?? false;
        hotelExistente.Downtown = hotel.Downtown ?? false;
        hotelExistente.Airpot = hotel.Airpot ?? false;
        hotelExistente.Highway = hotel.Highway ?? false;
        hotelExistente.Hospital = hotel.Hospital ?? false;
        hotelExistente.Coffee = hotel.Coffee ?? false;
        hotelExistente.Wifi = hotel.Wifi ?? false;
        hotelExistente.Swimming = hotel.Swimming ?? false;
        hotelExistente.Cleaning = hotel.Cleaning ?? false;
        hotelExistente.Gym = hotel.Gym ?? false;
        hotelExistente.Cnpj = hotel.Cnpj;
        hotelExistente.Razao = hotel.Razao; 
        hotelExistente.NomeRep = hotel.NomeRep;
        hotelExistente.TelRep = hotel.TelRep;
        hotelExistente.CpfRep = hotel.CpfRep;
        hotelExistente.EmailRep = hotel.EmailRep;

        await _context.SaveChangesAsync();

        return Retorno.Ok("Dados atualizados com sucesso!");
      }
      catch (Exception e)
      {
         return Retorno.Excecao(e, "Erro ao processar a solicitação.");
      }
    }


    public async Task<IRetorno> DeleteDetalhesFacade(string id)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return Retorno.Erro("Usuário não encontrado - email.");

        var user = await _utilsFacade.GetUserByEmail(userEmail);

        if (user == null || user.Id == Guid.Empty)
          return Retorno.Erro("Usuário não encontrado - id.");

        bool userHasPerm = await _utilsFacade.IsAdminOrManager(user.Id.ToString(), userEmail);

        if (!userHasPerm)
          return Retorno.Erro("Permissão do usuário não é admin/gerente deste hotel.");

        var hotel = await _context.Hotel.Where(u => u.Id.ToString() == id).FirstOrDefaultAsync();

        if (hotel == null)
          return Retorno.Erro("Hotel não encontrada.");

        // Exclui entidades filhas
        var permissoes = await _context.UsuarioPermissao.Where(x => x.DetalhesModelId == hotel.Id).ToListAsync();
        _context.UsuarioPermissao.RemoveRange(permissoes);

        var fotos = await _context.Photos.Where(x => x.DetalhesModelId == hotel.Id).ToListAsync();
        _context.Photos.RemoveRange(fotos);

        var contatos = await _context.Contacts.Where(x => x.DetalhesModelId == hotel.Id).ToListAsync();
        _context.Contacts.RemoveRange(contatos);

        _context.Hotel.Remove(hotel);
        await _context.SaveChangesAsync();

        return Retorno.Ok("Dados deletados com sucesso.");
      }
      catch (Exception e)
      {
        return Retorno.Excecao(e, "Erro ao processar a solicitação.");
      }
    }
  }
}


