using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using MediatR;
using Boilerplate.Api.Domain.Commands.Accounts;
using Boilerplate.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Boilerplate.Api.Domain.Services;
using SendGrid;
using Boilerplate.Api.Infrastructure.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Boilerplate.Api.Infrastructure;
using System.Text.Json.Serialization;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Boilerplate.Api.Infrastructure.Extensions;
using Boilerplate.Api.Infrastructure.Middleware;
using Boilerplate.Api;

var builder = WebApplication.CreateBuilder(args);
var appsettings = builder.Configuration.Get<Appsettings>();
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new DateTimeUtcConverter());
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Appsettings>(builder.Configuration);
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection(nameof(Appsettings.SendGrid)));
builder.Services.Configure<AuthorizationSettings>(builder.Configuration.GetSection(nameof(Appsettings.Authorization)));
builder.Services.Configure<RentalSettings>(builder.Configuration.GetSection(nameof(Appsettings.Rental)));
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IMailService, SendGridService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISendGridClient, SendGridClient>(serviceProvider => new SendGridClient(appsettings.SendGrid.ApiKey));
builder.Services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
builder.Services.AddMediatR(typeof(Boilerplate.Api.Infrastructure.Database.AppDbContext).Assembly); // use any type from Boilerplate.Api
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DbConnection)));
});

builder.Services.AddAuth(appsettings.Authorization);
builder.Services.AddCustomSwagger();
var app = builder.Build();

if (app.Environment.EnvironmentName != "Testing") // dont run this for integration tests
{
    using (var scope = app.Services.CreateScope())
    {
        await MigrateDb(scope);
        await SeedDb(scope);
        await EnsureDefaultAccounts(scope);
    }
}

async Task MigrateDb(IServiceScope scope)
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

async Task SeedDb(IServiceScope scope)
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
    await dbContext.SaveChangesAsync();
}

async Task EnsureDefaultAccounts(IServiceScope scope)
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
    var mediator = scope.ServiceProvider.GetService<IMediator>();
    var defaultAccountsToCreate = appsettings.DefaultAccounts;
    var existingAccounts = await dbContext.Accounts.Select(x => x.Email).ToListAsync();

    foreach (var defaultAcc in defaultAccountsToCreate)
    {
        if (existingAccounts.Contains(defaultAcc.Email, StringComparer.InvariantCultureIgnoreCase))
            continue;

        await mediator.Send(new CreateAccount.Command(defaultAcc.FullName, defaultAcc.Email));
    }
}

// ====== Pipeline ======
app.UseCustomSwagger();
app.UseHsts();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// this is necessary for the test project to access Program
public partial class Program { }