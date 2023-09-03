using Server.Services;
using Server;
using Server.Rabbitmq;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container
builder.Services.AddSingleton<RabbitMQClient>();
builder.Services.AddGrpc();

builder.Services.AddLogging(options => {
    options.AddConsole();
    options.SetMinimumLevel(LogLevel.Debug); 
});



var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ServiceAI>();

app.Run();

System.Console.WriteLine("Start");