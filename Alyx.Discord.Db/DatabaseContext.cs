using Alyx.Discord.Db.Models;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Alyx.Discord.Db;

public class DatabaseContext : DbContext, IDataProtectionKeyContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connStr = Environment.GetEnvironmentVariable(EnvironmentVariables.DbConnectionString);
            optionsBuilder.UseNpgsql(connStr, MapEnums).UseSnakeCaseNamingConvention();
            optionsBuilder.UseExceptionProcessor();
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.UseIdentityAlwaysColumns();

        base.OnModelCreating(modelBuilder);
    }

    private static void MapEnums(NpgsqlDbContextOptionsBuilder builder)
    {
        builder.MapEnum<HistoryType>();
    }

    #region DbSets

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public DbSet<Character> Characters { get; set; }
    public DbSet<InteractionData> InteractionDatas { get; set; }
    public DbSet<OptionHistory> OptionHistories { get; set; }

    #endregion
}