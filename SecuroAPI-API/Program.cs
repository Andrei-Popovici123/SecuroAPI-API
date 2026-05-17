using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecuroAPI.BusinessLogic.Services;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess;
using SecuroAPI.DataAccess.Entity;
using SecuroAPI.DataAccess.Repositories;
using SecuroAPI.DataAccess.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SecuroAPIDbContext");
builder.Services.AddDbContext<SecuroAPIDbContext>(options => options.UseSqlServer(connectionString));

//Identity
builder.Services.AddIdentityCore<IdentityUser>(options => { })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SecuroAPIDbContext>();


// Service and Repositories
builder.Services.AddScoped<IRepository<APIRegistry>,BaseRepository<APIRegistry>>();
builder.Services.AddScoped<IAPIRegistryService,APIRegistryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();