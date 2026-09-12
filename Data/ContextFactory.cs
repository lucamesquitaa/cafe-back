using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Turify.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            
            // Use a temporary connection string for migrations
            // This is only used at design-time when running migrations
            optionsBuilder.UseNpgsql("Host=localhost;Database=cafeteriasdb;Username=lucam;Password=X(y4M&.}@Mes6TZJ");
            
            return new Context(optionsBuilder.Options);
        }
    }
}
