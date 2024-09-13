using Alyx.Discord.Extensions;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Alyx.Discord.Bot.Services;

public class BotService(IConfiguration configuration) : IHostedService
{
    private readonly DiscordClient _client = CreateClient(configuration);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync();
    }

    private static DiscordClient CreateClient(IConfiguration configuration)
    {
        var token = configuration.GetGuardedConfiguration(EnvironmentVariables.BotToken);
        var builder = DiscordClientBuilder.CreateDefault(token, DiscordIntents.AllUnprivileged);

        return builder.Build();
    }
}