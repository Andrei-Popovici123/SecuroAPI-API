using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SecuroAPI_API.Handlers;
using SecuroAPI.BusinessLogic.Services;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Models;
using SecuroAPI.DataAccess;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories;
using SecuroAPI.DataAccess.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

//db context
var connectionString = builder.Configuration.GetConnectionString("SecuroAPIDbContext");
builder.Services.AddDbContext<SecuroAPIDbContext>(options => options.UseSqlServer(connectionString));

//Identity
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options => { })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SecuroAPIDbContext>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("JwtSettings: Key not configured");
}

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
    });

builder.Services.AddAuthorization();

// Service and Repositories
builder.Services.AddScoped<IRepository<APIRegistry>, BaseRepository<APIRegistry>>();
builder.Services.AddScoped<IAPIRegistryRepository, APIRegistryRepository>();
builder.Services.AddScoped<IAnomalyLogRepository,AnomalyLogRepository>();
builder.Services.AddScoped<IRatingRepository,RatingRepository>();
builder.Services.AddScoped<IScoreReportRepository,ScoreReportRepository>();

builder.Services.AddScoped<IAPIRegistryService, APIRegistryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IApiKeyValidatorService, ApiKeyValidatorService>();
builder.Services.AddScoped<IAnomalyLogService, AnomalyLogService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IScoreReportService, ScoreReportService>();

var app = builder.Build();

//Middleware Identity
app.MapGroup("api/defaultAuth").MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.EnablePersistAuthorization(); });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();