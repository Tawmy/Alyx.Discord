using Microsoft.Extensions.DependencyInjection;

namespace Alyx.Discord.Db;

public static class DependencyInjection
{
    public static void AddDbServices(this IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>();
    }
}