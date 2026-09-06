using System.Net;
using Microsoft.EntityFrameworkCore;
using SecuroAPI_MonitoringService;
using SecuroAPI_MonitoringService.Messaging;
using SecuroAPI.BusinessLogic.Services;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.DataAccess;
using SecuroAPI.DataAccess.Repositories;
using SecuroAPI.DataAccess.Repositories.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SecuroAPIDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("SecuroAPIDbContext")));

builder.Services.Configure<MonitoringOptions>(builder.Configuration.GetSection("Monitoring"));

builder.Services.AddScoped<IMonitoredEndpointRepository, MonitoredEndpointRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IMonitoringRunnerService, MonitoringRunnerService>();

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

builder.Services.AddHostedService<MonitoringWorker>();
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<MonitoringResultPublisher>();
builder.Services.AddSingleton<MonitoringRegisterConsumer>();
var host = builder.Build();
host.Run();
