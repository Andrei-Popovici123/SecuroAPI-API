using SecuroAPI_TestRunnerService;
using SecuroAPI_TestRunnerService.Logic;
using SecuroAPI_TestRunnerService.Logic.Interfaces;
using SecuroAPI_TestRunnerService.Messaging;
using SecuroAPI.Contracts.Connection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<TestResultPublisher>(); 
builder.Services.AddSingleton<TestJobConsumer>(); 
builder.Services.AddSingleton<IContainerRunner, DockerContainerRunner>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
