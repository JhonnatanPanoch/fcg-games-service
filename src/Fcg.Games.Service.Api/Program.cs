using Asp.Versioning;
using Fcg.Games.Service.Api.ApiConfigurations;
using Fcg.Games.Service.Api.ApiConfigurations.LogsConfig;
using Fcg.Games.Service.Api.Filters;
using Fcg.Games.Service.Api.Middlewares;
using Fcg.Games.Service.Application.ApiSettings;
using Fcg.Games.Service.Application.AppServices;
using Fcg.Games.Service.Application.ClientContracts.GamePurchase;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Interfaces;
using Fcg.Games.Service.Infra.Clients.GamePurchase;
using Fcg.Games.Service.Infra.Contexts;
using Fcg.Games.Service.Infra.Elastic.Configurations;
using Fcg.Games.Service.Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region database

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateGuidQueryParamsFilter>();
});

builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();

#region DIs

/// Applications
builder.Services.AddAbstractValidations();
builder.Services.AddScoped<IJogoAppService, JogoAppService>();
builder.Services.AddScoped<IPromocaoAppService, PromocaoAppService>();
builder.Services.AddScoped<IMetricaAppService, MetricaAppService>();
builder.Services.AddScoped<ISugestaoAppService, SugestaoAppService>();
builder.Services.AddScoped<IUsuarioAutenticadoAppService, UsuarioAutenticadoAppService>();

/// Domains

/// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

/// Clients
builder.Services.AddScoped<IGamePurchaseServiceClient, GamePurchaseServiceClient>();

#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    {
        Title = "Servi�o de Cat�logo de Jogos",
        Description= "Este servi�o � a fonte da verdade para tudo relacionado a jogos e promo��es.",
        Version = "v1"
    });

    c.EnableAnnotations();

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            new[] { "Bearer" }
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

#endregion

#region NewRelic
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown")
    .WriteTo.Console()
    .WriteTo.DurableHttpUsingFileSizeRolledBuffers(
        requestUri: "https://log-api.newrelic.com/log/v1",
        textFormatter: new NewRelicFormatter(),
        batchFormatter: new ArrayBatchFormatter(),
        httpClient: new NewRelicHttpClient())
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region Jwt
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtAppService, JwtAppService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey)),
        RoleClaimType = ClaimTypes.Role,
    };
});

builder.Services.AddAuthorization();
#endregion

#region HttpClientFactories
builder.Services.AddHttpClient("GamePurchaseService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:GamePurchaseApiUrl"]!);
});

#endregion

#region Elastic

ConfigureElasticClient.Configure(
    builder.Services, 
    builder.Configuration["Elasticsearch:Url"]!,
    builder.Configuration["Elasticsearch:Username"]!,
    builder.Configuration["Elasticsearch:Password"]!);

#endregion

var app = builder.Build();

#region Migrations

await app.ApplyMigrationsWithSeedsAsync();

#endregion

#region Middlewares

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

#endregion

app.Run();

public partial class Program { }