using NexusServergRPC.Services;
using NexusServergRPC.Server;
using NexusServergRPC.NexusContext;
using Microsoft.EntityFrameworkCore;
using NexusServergRPC.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NexusContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NexusConnectionString")));

builder.Services.AddGrpc();
builder.Services.AddSingleton<Nexus_Server>();
builder.Services.AddSingleton<NexusTokenIssuer>();

var app = builder.Build();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<NexusService>();
app.MapGrpcService<NexusDbService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
