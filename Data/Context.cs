using Microsoft.EntityFrameworkCore;
using Cafeteria.Models;
using CafeteriaModel = Cafeteria.Models.CafeteriaModel;

namespace Cafeteria.Data
{
  public class Context : DbContext
  {
    public DbSet<UserModel> Usuarios { get; set; }
    public DbSet<UsuarioPermissoes> UsuarioPermissao { get; set; }
    public DbSet<ErroLogModel> ErrorLogs { get; set; }
    public DbSet<CafeteriaModel> Cafeterias { get; set; }
    public DbSet<FotosDetalhesModel> Photos { get; set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Relacionamento 1:N entre UserModel e UsuarioPermissoes
      modelBuilder.Entity<UserModel>()
          .HasMany(h => h.Permissions)
          .WithOne(c => c.User)
          .HasForeignKey(c => c.UserModelId)
          .OnDelete(DeleteBehavior.Cascade);

      // Relacionamento 1:N entre Cafeteria e UsuarioPermissoes
      modelBuilder.Entity<CafeteriaModel>()
          .HasMany(h => h.Permissions)
          .WithOne(c => c.Cafeteria)
          .HasForeignKey(c => c.CafeteriaId)
          .OnDelete(DeleteBehavior.Cascade);

      // Relacionamento 1:N entre Cafeteria e FotosDetalhesModel
      modelBuilder.Entity<CafeteriaModel>()
          .HasMany<FotosDetalhesModel>()
          .WithOne(p => p.Cafeteria)
          .HasForeignKey(p => p.CafeteriaId)
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
