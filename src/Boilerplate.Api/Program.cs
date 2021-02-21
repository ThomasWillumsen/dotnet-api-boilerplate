using Boilerplate.Core.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Core.Database.Entities;

namespace Boilerplate.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
                MigrateDb(dbContext).Wait();
                SeedDb(dbContext).Wait();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static async Task MigrateDb(AppDbContext dbContext)
        {
            await dbContext.Database.MigrateAsync();
        }

        private static async Task SeedDb(AppDbContext dbContext)
        {
            if (await dbContext.Examples.AnyAsync())
                return;

            await dbContext.Examples.AddRangeAsync(
                new ExampleEntity[]{
                    new ExampleEntity{
                        Name = "test1",
                        Type = TypeEnum.A

                    },
                    new ExampleEntity{
                        Name = "test2",
                        Type = TypeEnum.B
                    },
                    new ExampleEntity{
                        Name = "test3",
                        Type = TypeEnum.C
                    }
                }
            );

            await dbContext.SaveChangesAsync();
        }
    }
}