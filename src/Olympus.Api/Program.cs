
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Olympus.Api;
using Olympus.Application;
using Olympus.Infrastructure.Ai;

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

builder.Services
  .AddOlympusServices()
  .AddGrpcServerServices()
  .AddAiServices(builder.Configuration);

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
