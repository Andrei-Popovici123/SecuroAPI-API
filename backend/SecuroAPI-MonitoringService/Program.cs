using SecuroAPI_MonitoringService;
using SecuroAPI_MonitoringService.Messaging;
using SecuroAPI.Contracts.Connection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<MonitoringResultPublisher>();
builder.Services.AddSingleton<MonitoringRegisterConsumer>();
var host = builder.Build();
host.Run();
