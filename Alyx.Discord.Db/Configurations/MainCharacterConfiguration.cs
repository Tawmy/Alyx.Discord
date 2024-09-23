using Alyx.Discord.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alyx.Discord.Db.Configurations;

public class MainCharacterConfiguration : IEntityTypeConfiguration<MainCharacter>
{
    public void Configure(EntityTypeBuilder<MainCharacter> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.DiscordId).IsUnique(); // one main character per discord id

        builder.Property(x => x.CharacterId).HasMaxLength(10);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Code).HasMaxLength(32).IsFixedLength();
    }
}