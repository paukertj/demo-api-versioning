using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class SettingsEntityConfiguration : IEntityTypeConfiguration<SettingsEntity>
{
    public void Configure(EntityTypeBuilder<SettingsEntity> builder)
    {
        builder
            .ToTable("settings");

        builder
            .HasKey(settings => new { settings.Key, settings.ValueVersion, settings.SchemaVersion });

        builder
            .Property(settings => settings.Key)
            .HasMaxLength(256)
            .IsRequired();
    }
}
