using Boilerplate.Core.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;

namespace Boilerplate.Core.Database
{
    public interface IAppDbContext
    {
        DbSet<ExampleEntity> Examples { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class AppDbContext : DbContext, IAppDbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext()
        {
        }

        public DbSet<ExampleEntity> Examples { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ExampleEntity>()
                .ToTable("Examples")
                .Property(x => x.CreatedDate)
                    .HasDefaultValueSql("getutcdate()");
        }
    }
}