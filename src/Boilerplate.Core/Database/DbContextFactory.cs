using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Boilerplate.Core.Database
{
    /// <summary>
    /// Used upon Updating database from local development using the EF cli, which you should basically not be doing. 
    /// You should let the app take care of applying migrations upon startup.
    /// The connection string here is not used when running the application and can as such be left with the dev/local credentials or even empty.
    /// If in doubt, read up on the official documentation of a DbContextFactory
    /// </summary>
    public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Example-db;Trusted_Connection=True;MultipleActiveResultSets=true;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}