using System.Data;
using Alyx.Discord.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Alyx.Discord.Core.Extensions;

public static class ServiceProviderExtension
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await dbContext.Database.MigrateAsync();

        if (dbContext.Database.GetDbConnection() is NpgsqlConnection npgsqlConnection)
        {
            if (npgsqlConnection.State is not ConnectionState.Open)
            {
                await npgsqlConnection.OpenAsync();
            }

            try
            {
                await npgsqlConnection.ReloadTypesAsync();
            }
            finally
            {
                await npgsqlConnection.CloseAsync();
            }
        }
    }
}