using Alyx.Discord.Db.Models;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Db;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    #region DbSets

    public DbSet<Character> Characters { get; set; }

    #endregion

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
}