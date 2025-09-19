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
  public class HotelFacade : IHotelFacade
  {
    private readonly Context _context;
    private readonly IRabbitMqProducer _producer;
    private readonly GoogleAuthService _googleAuthService;

    public HotelFacade(Context context, IRabbitMqProducer producer, GoogleAuthService googleAuthService)
    {
      _context = context;
      _producer = producer;
      _googleAuthService = googleAuthService;
    }

    public async Task<IEnumerable<GetAllHoteis>> GetAllFacade()
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

        return hoteis;
      }
      catch (Exception e)
      {
        return null;
      }
    }
    public async Task<GetDetalheById> GetDetalhesFacade(string hotelId)
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

        return hotel;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public async Task<DetalhesModel> GetDetalhesFacadeByManager(string hotelId)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return null;

        bool userHasPerm = await UserIsAdminOrManager(hotelId, userEmail);

        if (!userHasPerm)
          return null;

        var hotel = await _context.Hotel.Where(u => u.Id.ToString() == hotelId)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();

        return hotel;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public async Task<IEnumerable<GetAllHoteis>> GetDetalhesUserFacade()
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return null;

        var userId = await _context.Usuarios
            .Where(u => u.Email == userEmail)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (userId == Guid.Empty)
          return null;

        var hotelIds = await _context.UsuarioPermissao
                                  .Where(up => up.UserModelId == userId &&
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

        return hoteis;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public async Task<IActionResult> PostDetalhesFacade(DetalhesModel hotel)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return null;

        var userId = await _context.Usuarios
            .Where(u => u.Email == userEmail)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (userId == Guid.Empty)
          return null;

        // Verifica se já existe um hotel com a mesma URL
        var urlHotel = await _context.Hotel.FirstOrDefaultAsync(u => u.Url == hotel.Url);
        if (urlHotel != null)
        {
          return new BadRequestObjectResult("Já existe um hotel cadastrado com esta URL.");
        }

        var novoId = Guid.NewGuid();

        var permissao = new UsuarioPermissoes
        {
          Id = Guid.NewGuid(),
          DetalhesModelId = novoId,
          UserModelEmail = userEmail,
          UserModelId = userId,
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

        return new OkResult();
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(e.Message);
      }
    }


    public async Task<IActionResult> PutDetalhesFacade(DetalhesModel hotel, string id)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return null;

        bool userHasPerm = await UserIsAdminOrManager(id, userEmail);

        if (!userHasPerm)
          return new BadRequestObjectResult("Permissão do usuário não é admin/gerente deste hotel."); ;

        var userId = await _context.Usuarios
            .Where(u => u.Email == userEmail)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (userId == Guid.Empty)
          return new NotFoundObjectResult("Usuário não encontrado.");

        var hotelId = Guid.Parse(id);
        var hotelExistente = await _context.Hotel
            .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotelExistente == null)
          return new NotFoundObjectResult("Hotel não encontrado.");

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

        return new OkResult();
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(e.Message);
      }
    }


    public async Task<IActionResult> DeleteDetalhesFacade(string id)
    {
      try
      {
        var userEmail = _googleAuthService.GetUserEmailFromToken();

        if (string.IsNullOrEmpty(userEmail))
          return new BadRequestObjectResult("Usuário não encontrado.");

        bool userHasPerm = await UserIsAdminOrManager(id, userEmail);

        if (!userHasPerm)
          return new BadRequestObjectResult("Permissão do usuário não é admin/gerente deste hotel.");

        var hotel = await _context.Hotel.Where(u => u.Id.ToString() == id).FirstOrDefaultAsync();
        if (hotel == null)
        {
          return new BadRequestObjectResult("Hotel não encontrada.");

        }

        // Exclui entidades filhas
        var permissoes = await _context.UsuarioPermissao.Where(x => x.DetalhesModelId == hotel.Id).ToListAsync();
        _context.UsuarioPermissao.RemoveRange(permissoes);

        var fotos = await _context.Photos.Where(x => x.DetalhesModelId == hotel.Id).ToListAsync();
        _context.Photos.RemoveRange(fotos);

        var contatos = await _context.Contacts.Where(x => x.DetalhesModelId == hotel.Id).ToListAsync();
        _context.Contacts.RemoveRange(contatos);

        _context.Hotel.Remove(hotel);
        await _context.SaveChangesAsync();
        return new OkResult();
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(e);
      }
    }

    public async Task<bool> UserIsAdminOrManager(string hotelId, string userEmail)
    {
      var userId = _context.Usuarios
          .Where(u => u.Email == userEmail)
          .Select(x => x.Id)
          .FirstOrDefault();
      
      if (userId == Guid.Empty)
        return false;

      var hotelGuid = Guid.Parse(hotelId);

      var user = await _context.UsuarioPermissao.FirstOrDefaultAsync(x => x.UserModelId == userId && x.DetalhesModelId == hotelGuid);

      bool userIsAdmin = user?.Role == RoleUserModel.Admin || user?.Role == RoleUserModel.Manager || user?.Role == RoleUserModel.Turify;

      return userIsAdmin;
    }
  }
}


