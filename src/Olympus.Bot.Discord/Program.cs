using Olympus.Bot.Discord;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(logging =>
{
  _ = logging.ClearProviders()
      .AddConsole()
      .AddDebug()
      .AddEventSourceLogger();
});

builder.Services.AddDiscordServices();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.AddDiscordServices();

await host.RunAsync();
