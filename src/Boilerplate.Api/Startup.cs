using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Boilerplate.Api.Domain.Services;
using SendGrid;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.Authorization;
using Boilerplate.Api.Infrastructure;
using Boilerplate.Api.Infrastructure.Middleware;
using Boilerplate.Api.Infrastructure.Extensions;
using Boilerplate.Api.Infrastructure.ErrorHandling;

namespace Boilerplate.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureIOptions(Configuration);
            var appsettings = Configuration.Get<Appsettings>();
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration.GetConnectionString("DbConnection"));
            });

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IMailService, SendGridService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ISendGridClient, SendGridClient>(serviceProvider => new SendGridClient(appsettings.SendGrid.ApiKey));
            services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
            services.AddMediatR(typeof(Boilerplate.Api.Infrastructure.Database.AppDbContext).Assembly); // use any type from Boilerplate.Api

            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new DateTimeUtcConverter());
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions();

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "Frontend/build"; });
            services.AddAuth(appsettings.Authorization);
            services.AddCustomSwagger();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCustomSwagger();
            app.UseHsts();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSpa(spa => { spa.Options.SourcePath = "Frontend"; });
        }
    }
}
