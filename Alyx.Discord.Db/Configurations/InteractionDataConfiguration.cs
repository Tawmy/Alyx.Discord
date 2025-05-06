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

        // Guid Version 7 is 36 characters, but previous logic created 32 character long strings
        // -> cannot use fixed length
        builder.Property(x => x.Key).HasMaxLength(36);

        builder.Property(x => x.Value).HasColumnType("jsonb").HasMaxLength(2000);

        builder.Property(x => x.Type).HasMaxLength(256);
    }
}