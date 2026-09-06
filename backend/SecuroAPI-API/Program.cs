using System.Net;
using System.Text;
using DnsClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SecuroAPI_API.Handlers;
using SecuroAPI_API.Messaging;
using SecuroAPI.BusinessLogic.Services;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.BusinessLogic.Services.Publisher;
using SecuroAPI.Common.Models;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.DataAccess;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories;
using SecuroAPI.DataAccess.Repositories.Interfaces;


DotNetEnv.Env.Load();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

//CORS
const string corsPolicy = "SecuroApiCors";

var origins = builder.Configuration
                  .GetSection("Cors:AllowedOrigins").Get<string[]>()
              ?? throw new InvalidOperationException("Cors:AllowedOrigins not configured");

builder.Services.AddCors(o => o.AddPolicy(corsPolicy, p => p
    .WithOrigins(origins)
    .AllowAnyHeader()
    .AllowAnyMethod()));


//db context
var connectionString = builder.Configuration.GetConnectionString("SecuroAPIDbContext");
builder.Services.AddDbContext<SecuroAPIDbContext>(options => options.UseSqlServer(connectionString));

//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
    {
        o.Lockout.MaxFailedAccessAttempts = 5;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        o.Lockout.AllowedForNewUsers = true;
        o.Password.RequiredLength = 12;
        o.Password.RequireNonAlphanumeric = false;
        o.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<SecuroAPIDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("JwtSettings: Key not configured");
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
        options.EventsType = typeof(TokenRevocationEvents);
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ApprovedUser", p => p.RequireClaim("status", "Approved"));
});

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddHostedService<TestResultConsumer>();
builder.Services.AddHostedService<MonitoringResultConsumer>();
builder.Services.AddSingleton<ITestJobPublisher, TestJobPublisher>();
builder.Services.AddSingleton<IMonitoringRegisterPublisher, MonitoringRegisterPublisher>();
builder.Services.AddHostedService<StuckJobClearer>();
builder.Services.AddSingleton<ILookupClient>(_ => new LookupClient(
    new LookupClientOptions { UseCache = false, Timeout = TimeSpan.FromSeconds(5) }));
builder.Services.AddScoped<TokenRevocationEvents>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



// Service and Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRepository<APIRegistry>, BaseRepository<APIRegistry>>();
builder.Services.AddScoped<IAPIRegistryRepository, APIRegistryRepository>();
builder.Services.AddScoped<IAnomalyLogRepository, AnomalyLogRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IScoreReportRepository, ScoreReportRepository>();
builder.Services.AddScoped<ITestConfigRepository, TestConfigRepository>();
builder.Services.AddScoped<ITestJobRepository, TestJobRepository>();
builder.Services.AddScoped<IMonitoredEndpointRepository, MonitoredEndpointRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();

builder.Services.AddScoped<IAPIRegistryService, APIRegistryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IApiKeyValidatorService, ApiKeyValidatorService>();
builder.Services.AddScoped<IAnomalyLogService, AnomalyLogService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IScoreReportService, ScoreReportService>();
builder.Services.AddScoped<ITestConfigService, TestConfigService>();
builder.Services.AddScoped<IAdministrationService, AdministrationService>();
builder.Services.AddScoped<ITestJobService, TestJobService>();
builder.Services.AddScoped<ITestJobCrudService, TestJobCrudService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IMonitoredEndpointService, MonitoredEndpointService>();
builder.Services.AddScoped<IMonitoringDashboardService, MonitoringDashboardService>();
builder.Services.AddHttpClient<IProbeService, ProbeService>(c =>
    {
        c.Timeout = TimeSpan.FromSeconds(10);
        c.DefaultRequestHeaders.UserAgent.ParseAdd("SecuroAPI-Monitor/1.0");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.All
    });


var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SecuroAPIDbContext>();
    db.Database.Migrate();
}



app.Use(async (ctx, next) =>
{
    var h = ctx.Response.Headers;
    h["X-Content-Type-Options"] = "nosniff";
    h["X-Frame-Options"] = "DENY";
    h["Referrer-Policy"] = "no-referrer";
    h["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
    
    if (!app.Environment.IsDevelopment())
        h["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
    await next();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.EnablePersistAuthorization(); });
}


app.UseHttpsRedirection();
app.UseRouting(); 
app.UseCors(corsPolicy);
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();