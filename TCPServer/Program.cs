using TCPServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TcpServerService>();
var app = builder.Build();

app.Run();
