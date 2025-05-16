using Olympus.Bot.Discord.Core;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .AddEnvironmentVariables();

builder.Services.AddLogging(logging =>
{
  _ = logging.ClearProviders()
      .AddConsole()
      .AddDebug()
      .AddEventSourceLogger();
});

builder.Services.AddOptions<DiscordSettings>()
    .Bind(builder.Configuration.GetSection("Discord"))
    .ValidateDataAnnotations();
builder.Services.AddSingleton<DiscordGateway>();
builder.Services.AddHostedService<DiscordBotWorker>();

var host = builder.Build();

await host.RunAsync();
