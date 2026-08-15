using SecuroAPI_TestRunnerService;
using SecuroAPI_TestRunnerService.Messaging;
using SecuroAPI.Contracts.Connection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<TestResultPublisher>(); 
var host = builder.Build();
host.Run();
