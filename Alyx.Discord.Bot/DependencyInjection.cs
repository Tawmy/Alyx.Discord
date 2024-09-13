using Alyx.Discord.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alyx.Discord.Bot;

public static class DependencyInjection
{
    public static void AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<BotService>();
    }
}