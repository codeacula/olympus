using Olympus.Bot.Discord;
using Olympus.Bot.Discord.Core;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(logging =>
{
  _ = logging.ClearProviders()
      .AddConsole()
      .AddDebug()
      .AddEventSourceLogger();
});

builder.Services.AddDiscordServices();

builder.Services.AddHostedService<DiscordBotWorker>();

var host = builder.Build();

host.AddDiscordServices();

await host.RunAsync();
