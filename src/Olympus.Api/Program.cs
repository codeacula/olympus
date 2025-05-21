using System.Net.Security;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Olympus.Api;
using Olympus.Application;
using Olympus.Application.Common.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new() { Title = "Olympus API", Version = "v1" });
  c.EnableAnnotations();
});

builder.Services.AddHealthChecks()
    .AddCheck<SelfHealthCheck>("OlympusApi");

// Configure GrpcChannel explicitly
builder.Services.AddSingleton(services =>
{
  var options = services.GetRequiredService<IOptions<GrpcHostConfig>>().Value;
  var loggerFactory = services.GetRequiredService<ILoggerFactory>();

  // Create channel options
  var channelOptions = new GrpcChannelOptions
  {
    LoggerFactory = loggerFactory,
    ServiceProvider = services,
    HttpHandler = new SocketsHttpHandler
    {
      MaxConnectionsPerServer = 100,
      PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
      EnableMultipleHttp2Connections = true,
      SslOptions = new SslClientAuthenticationOptions
      {
        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
        RemoteCertificateValidationCallback = (_, _, _, _) => true,
      },
    },
  };

  // Construct the appropriate URI based on config
  var scheme = options.UseHttps ? "https" : "http";
  var address = $"{scheme}://{options.Host}:{options.Port}";
  Console.WriteLine($"Creating gRPC channel with address: {address}");

  return GrpcChannel.ForAddress(address, channelOptions);
});

builder.Services.AddOlympusServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
_ = app.MapOpenApi();
_ = app.UseSwagger();
_ = app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => "Welcome to Olympus API!");
app.MapHealthChecks("/health", new HealthCheckOptions
{
  Predicate = _ => true, // Include all checks
});

app.AddOlympusServices();

app.UseHttpsRedirection();

app.Run();

/// <summary>
/// Program class made public and partial to enable integration testing with WebApplicationFactory
/// </summary>
public partial class Program;
