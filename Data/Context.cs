using Microsoft.EntityFrameworkCore;
using Turify.Models;

namespace Turify.Data
{
  public class Context : DbContext
  {
    public DbSet<UserModel> Usuarios { get; set; }
    public DbSet<UsuarioPermissoes> UsuarioPermissao { get; set; }
    public DbSet<ErroLogModel> ErrorLogs { get; set; }
    public DbSet<Cafeteria> Cafeterias { get; set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Relacionamento 1:N entre UserModel e UsuarioPermissoes
      modelBuilder.Entity<UserModel>()
          .HasMany(h => h.Permissions)
          .WithOne()
          .HasForeignKey(c => c.UserModelId)
          .OnDelete(DeleteBehavior.Cascade);

      // Relacionamento 1:N entre Cafeteria e UsuarioPermissoes
      modelBuilder.Entity<Cafeteria>()
          .HasMany(h => h.Permissions)
          .WithOne()
          .HasForeignKey(c => c.CafeteriaId)
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
