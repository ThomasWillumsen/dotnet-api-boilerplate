using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;
using System;
using MediatR;
using Boilerplate.Api.Domain.Commands.Accounts;
using Boilerplate.Api.Infrastructure.Database;

namespace Boilerplate.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                await MigrateDb(scope);
                await SeedDb(scope);
                await EnsureDefaultAccounts(scope);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static async Task MigrateDb(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static async Task SeedDb(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            await dbContext.SaveChangesAsync();
        }

        private static async Task EnsureDefaultAccounts(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            var defaultAccountsToCreate = scope.ServiceProvider.GetService<IOptions<Appsettings>>().Value.DefaultAccounts;
            var existingAccounts = await dbContext.Accounts.Select(x => x.Email).ToListAsync();

            foreach (var defaultAcc in defaultAccountsToCreate)
            {
                if (existingAccounts.Contains(defaultAcc.Email, StringComparer.InvariantCultureIgnoreCase))
                    continue;

                await mediator.Send(new CreateAccount.Command(defaultAcc.FullName, defaultAcc.Email));
            }
        }
    }
}