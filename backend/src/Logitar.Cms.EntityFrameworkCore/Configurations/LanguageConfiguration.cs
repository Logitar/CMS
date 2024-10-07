using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.Domain.Shared;
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

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.IsDefault); // ISSUE: https://github.com/Logitar/CMS/issues/2
    builder.HasIndex(x => x.LCID).IsUnique();
    builder.HasIndex(x => x.Code);
    builder.HasIndex(x => x.CodeNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.EnglishName);
    builder.HasIndex(x => x.NativeName);

    builder.Property(x => x.Code).HasMaxLength(LocaleUnit.MaximumLength);
    builder.Property(x => x.CodeNormalized).HasMaxLength(LocaleUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EnglishName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.NativeName).HasMaxLength(byte.MaxValue);
  }
}
