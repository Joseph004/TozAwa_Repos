using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Principal;
using Tozawa.Attachment.Svc.Clients;
using Tozawa.Attachment.Svc.Configuration;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tozawa.Attachment.Svc.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Tozawa.Attachment.Svc;

public class Startup
{
    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureAppSettings<AppSettings>(Configuration.GetSection("AppSettings"));
        var jwtSettings = Configuration.GetSection("AppSettings:JWTSettings");
        services.AddSingleton(Configuration);
        services.AddApplicationInsightsTelemetry();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddAuthentication()
               .AddJwtBearer("tzuserauthentication", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["validIssuer"],
        ValidAudience = jwtSettings["validAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
    };
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
               {
                   opt.Audience = AppSettings.AAD.ResourceId;
                   opt.Authority = $"{AppSettings.AAD.Instance}{AppSettings.AAD.TenantId}";
               });

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        services.AddMediatR(typeof(Startup));
        services
            .AddMvc(options => { options.Filters.Add(new RequireHttpsAttribute()); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>());
        services
            .RegisterValidationService()
            .RegisterClients()
            .RegisterConverters()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>))
            .AddTransient(typeof(IRequestPreProcessor<>), typeof(ValidationRequestPreProcessor<>));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

        services.AddControllers();
        // For Entity Framework  
        services.AddDbContext<AttachmentContext>(options => options.UseSqlServer(AppSettings.ConnectionStrings.Sql,
                                                                    x =>
                                                                    {
                                                                        x.MigrationsHistoryTable("MigrationHistory", "Attachment");
                                                                        x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), new List<int>());
                                                                    }));
        services.AddSingleton(AppSettings);

        services.AddHealthChecks();
        services.AddCors((opt) =>
        {
            opt.AddPolicy("ALL", (o) =>
            {
                o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            UpdateDatabase(app);
        }

        app.UseCors("ALL");

        var options = new RewriteOptions()
            .AddRedirectToHttps(StatusCodes.Status301MovedPermanently, 44384);
        app.UseRewriter(options);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private void UpdateDatabase(IApplicationBuilder app)
    {
        using var serviceScope = app
            .ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        var appSettings = serviceScope.ServiceProvider.GetService<AppSettings>();
        if (appSettings == null) return;
        var options = new DbContextOptionsBuilder<AttachmentContext>()
            .UseSqlServer(appSettings.ConnectionStrings.MigrationSql,
                x =>
                {
                    x.MigrationsHistoryTable("MigrationHistory", "Attachment");
                    x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), new List<int>());
                });
        using var context = new AttachmentContext(options.Options);
        context.Database.SetCommandTimeout(600);
        context.Database.Migrate();
    }
}
