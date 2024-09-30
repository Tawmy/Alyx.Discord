using Alyx.Discord.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alyx.Discord.Db.Configurations;

internal class InteractionDataConfiguration : IEntityTypeConfiguration<InteractionData>
{
    public void Configure(EntityTypeBuilder<InteractionData> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Key).IsUnique();

        builder.Property(x => x.Key).HasMaxLength(32).IsFixedLength();

        builder.Property(x => x.Value).HasColumnType("jsonb").HasMaxLength(2000);

        builder.Property(x => x.Type).HasMaxLength(256);
    }
}