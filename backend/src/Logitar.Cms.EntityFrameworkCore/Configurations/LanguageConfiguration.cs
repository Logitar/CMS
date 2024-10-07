using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class LanguageConfiguration : AggregateConfiguration<LanguageEntity>, IEntityTypeConfiguration<LanguageEntity>
{
  public override void Configure(EntityTypeBuilder<LanguageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.Languages.Table.Table ?? string.Empty, CmsDb.Languages.Table.Schema);
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.IsDefault);
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => x.LocaleNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.EnglishName);
    builder.HasIndex(x => x.NativeName);

    builder.Property(x => x.Locale).HasMaxLength(16);
    builder.Property(x => x.LocaleNormalized).HasMaxLength(16);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EnglishName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.NativeName).HasMaxLength(byte.MaxValue);
  }
}
