using Microsoft.EntityFrameworkCore;
using Turify.Models;

namespace Turify.Data
{
  public class Context : DbContext
  {
    public DbSet<DetalhesModel> Hotel { get; set; }
    public DbSet<UserModel> Usuarios { get; set; }
    public DbSet<UsuarioPermissoes> UsuarioPermissao { get; set; }
    public DbSet<FotosDetalhesModel> Photos { get; set; }
    public DbSet<ContatosModel> Contacts { get; set; }
    public DbSet<QuartosModel> Quartos { get; set; }
    public DbSet<CategoryQuarto> CategoryQuarto { get; set; }
    public DbSet<ErroLogModel> ErrorLogs { get; set; }
    public DbSet<QuartoAvailable> QuartoAvailable { get; set; }
    public DbSet<QuartoReservas> QuartoReservas { get; set; }
    public DbSet<Hospedes> Hospedes { get; set; }
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Relacionamento 1:N entre QuartoReservas e Hospedes
      modelBuilder.Entity<Hospedes>()
        .HasOne(h => h.Reservation)
        .WithMany(r => r.Hospede)
        .HasForeignKey(h => h.ReservationId)
        .OnDelete(DeleteBehavior.Cascade);

      //catalogo reutilizavel chamado CategoryQuarto
      modelBuilder.Entity<QuartosModel>()
      .HasMany(q => q.Category)
      .WithMany()
      .UsingEntity<Dictionary<string, object>>(
        "QuartoCategory",
        j => j.HasOne<CategoryQuarto>().WithMany().HasForeignKey("CategoryQuartoId").OnDelete(DeleteBehavior.Restrict),
        j => j.HasOne<QuartosModel>().WithMany().HasForeignKey("QuartosModelId").OnDelete(DeleteBehavior.Cascade)
      );

      // Relacionamento 1:N entre DetalhesModel e FotosDetalhesModel
      modelBuilder.Entity<DetalhesModel>()
          .HasMany(h => h.Photos)
          .WithOne()
          .HasForeignKey(p => p.DetalhesModelId)
          .OnDelete(DeleteBehavior.Cascade);

      // Relacionamento 1:N entre QuartosModel e FotosDetalhesModel
      modelBuilder.Entity<QuartosModel>()
          .HasMany(h => h.Photos)
          .WithOne()
          .HasForeignKey(p => p.QuartosModelId)
          .OnDelete(DeleteBehavior.Cascade);


      // Relacionamento 1:N entre DetalhesModel e ContatosModel
      modelBuilder.Entity<DetalhesModel>()
          .HasMany(h => h.Contacts)
          .WithOne()
          .HasForeignKey(c => c.DetalhesModelId)
          .OnDelete(DeleteBehavior.Cascade);

      // Realacionamento 1:N
      modelBuilder.Entity<DetalhesModel>()
          .HasMany(h => h.Permissions)
          .WithOne()
          .HasForeignKey(c => c.DetalhesModelId)
          .OnDelete(DeleteBehavior.Cascade);

      // Realacionamento 1:N
      modelBuilder.Entity<UserModel>()
          .HasMany(h => h.Permissions)
          .WithOne()
          .HasForeignKey(c => c.UserModelId)
          .OnDelete(DeleteBehavior.Cascade);

      //reservas
      modelBuilder.Entity<QuartosModel>()
         .HasMany(h => h.Disponibilidade)
         .WithOne()
         .HasForeignKey(p => p.QuartosModelId)
         .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<QuartosModel>()
         .HasMany(h => h.Reservas)
         .WithOne()
         .HasForeignKey(p => p.QuartosModelId)
         .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
