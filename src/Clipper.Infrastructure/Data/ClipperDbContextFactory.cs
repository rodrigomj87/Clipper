using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Clipper.Infrastructure.Data
{
    /// <summary>
    /// Factory para criação do ClipperDbContext em tempo de design (migrations)
    /// </summary>
    public class ClipperDbContextFactory : IDesignTimeDbContextFactory<ClipperDbContext>
    {
        public ClipperDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClipperDbContext>();
            // String de conexão para banco já existente
            var connectionString = "Server=127.0.0.1,1433;Database=ClipperTestDb;User Id=sa;Password=Tec@123!;MultipleActiveResultSets=true;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connectionString);
            return new ClipperDbContext(optionsBuilder.Options);
        }
    }
}
