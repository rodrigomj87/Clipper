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
            var connectionString = Environment.GetEnvironmentVariable("CLIPPER_DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string not found. Please set the CLIPPER_DB_CONNECTION_STRING environment variable.");
            }
            optionsBuilder.UseSqlServer(connectionString);
            return new ClipperDbContext(optionsBuilder.Options);
        }
    }
}
