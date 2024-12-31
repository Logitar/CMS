using Logitar.Cms.Infrastructure.Entities;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.Infrastructure.Configurations;

internal class LanguageConfiguration : AggregateConfiguration<LanguageEntity>, IEntityTypeConfiguration<LanguageEntity>
{
  public override void Configure(EntityTypeBuilder<LanguageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.Languages.Table.Table ?? string.Empty, CmsDb.Languages.Table.Schema);
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.IsDefault);
    builder.HasIndex(x => x.LCID);
    builder.HasIndex(x => x.Code);
    builder.HasIndex(x => x.CodeNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.EnglishName);
    builder.HasIndex(x => x.NativeName);

    builder.Property(x => x.Code).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.CodeNormalized).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.EnglishName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.NativeName).HasMaxLength(DisplayName.MaximumLength);
  }
}
