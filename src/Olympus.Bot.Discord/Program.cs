using Olympus.Bot.Discord.Commands;
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

var botToken = new BotToken(builder.Configuration["Discord:BotToken"] ?? "");

builder.Services.AddOptions<DiscordSettings>()
    .Bind(builder.Configuration.GetSection("Discord"))
    .ValidateDataAnnotations();
builder.Services
  .AddSingleton<DiscordGateway>()
  .AddSingleton(new GatewayClientConfiguration
  {
    Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent
  })
  .AddSingleton<IEntityToken>(botToken);
builder.Services
  .AddTransient<GatewayClient>()
  .AddTransient<TestInteractionModule>();
builder.Services.AddHostedService<DiscordBotWorker>();

var host = builder.Build();

await host.RunAsync();
