using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tozawa.Auth.Svc.Configurations;
using Tozawa.Auth.Svc.Context;
using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Models.Converters;
using Tozawa.Auth.Svc.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

var appSettings = builder.Services.ConfigureAppSettings<AppSettings>(configuration.GetSection("AppSettings"));

builder.Services.AddAuthentication().AddJwtBearer("tzuserauthentication", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = appSettings.JWTSettings.ValidIssuer,
        ValidAudience = appSettings.JWTSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTSettings.SecurityKey))
    };
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
               {
                   opt.Audience = appSettings.AAD.ResourceId;
                   opt.Authority = $"{appSettings.AAD.Instance}{appSettings.AAD.TenantId}";
               });
builder.Services.AddAuthorization(options =>
           {
               options.DefaultPolicy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .AddAuthenticationSchemes("tzuserauthentication", JwtBearerDefaults.AuthenticationScheme)
                   .Build();
           });
builder.Services.AddDataProtection();

builder.Services.AddCors(options => options.AddPolicy("TozAwaCorsPolicyBff", builder =>
   {
       var origins = appSettings.CorsOrigins.Split(",");
       builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
   }));

builder.Services.AddControllers().AddXmlSerializerFormatters().AddNewtonsoftJson();

// For Entity Framework  
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.Sql));

// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentUserConverter, CurrentUserConverter>();
builder.Services.AddScoped<IDataProtectionProviderService, DataProtectionProviderService>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();
builder.Services.AddScoped<ICurrentCountry, CurrentCountry>();
builder.Services.Configure<CookiePolicyOptions>(options =>
                       {
                           options.CheckConsentNeeded = context => true;
                           options.MinimumSameSitePolicy = SameSiteMode.None;
                       });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "myauth";
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            });

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
    options.Filters.Add(new RequireHttpsAttribute());
}).AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Program>())
 .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                }); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ALL");
app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseCors("TozAwaCorsPolicyBff");
app.UseAuthentication();

app.UseAuthorization();

app.UseMvc();
app.Run();
