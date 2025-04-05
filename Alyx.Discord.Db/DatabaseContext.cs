using Alyx.Discord.Db.Models;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
            optionsBuilder.UseNpgsql(connStr).UseSnakeCaseNamingConvention();
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

    #region DbSets

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public DbSet<Character> Characters { get; set; }
    public DbSet<InteractionData> InteractionDatas { get; set; }

    #endregion
}