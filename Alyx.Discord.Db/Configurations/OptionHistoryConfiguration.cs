using Alyx.Discord.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alyx.Discord.Db.Configurations;

public class OptionHistoryConfiguration : IEntityTypeConfiguration<OptionHistory>
{
    public void Configure(EntityTypeBuilder<OptionHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.HistoryType).HasMaxLength(32);

        builder.Property(x => x.Value).HasMaxLength(256);
    }
}