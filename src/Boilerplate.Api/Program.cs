
using System.Text.Json.Serialization;
using Boilerplate.Api;
using Boilerplate.Api.Domain.Commands.Accounts;
using Boilerplate.Api.Domain.PipelineBehaviours;
using Boilerplate.Api.Domain.Services;
using Boilerplate.Api.Infrastructure;
using Boilerplate.Api.Infrastructure.Authorization;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Boilerplate.Api.Infrastructure.Middleware;
using Boilerplate.Api.Infrastructure.Swagger;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var appsettings = builder.Configuration.Get<Appsettings>()!;

// init serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Default", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new DateTimeUtcConverter());
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions();
builder.Services.Configure<Appsettings>(builder.Configuration);
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection(nameof(Appsettings.SendGrid)));
builder.Services.Configure<AuthorizationSettings>(builder.Configuration.GetSection(nameof(Appsettings.Authorization)));
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ISendGridClientFacade, SendGridClientFacade>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISendGridClient, SendGridClient>(serviceProvider => new SendGridClient(appsettings.SendGrid.ApiKey));
builder.Services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseMySql(
        builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DbConnection)),
        new MySqlServerVersion(new Version(8, 0, 34))); // found in phpmyadmin by executing SELECT VERSION();
});
builder.Services.AddAuth(appsettings.Authorization);
builder.Services.AddCustomSwagger();
var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.EnvironmentName != "Testing") // dont run this for integration tests
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
        var mediator = scope.ServiceProvider.GetService<IMediator>()!;
        await dbContext.Database.MigrateAsync();
        await mediator.Send(new EnsureDefaultAdminAccounts.Command());
    }
}

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