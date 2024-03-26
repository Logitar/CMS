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

    builder.ToTable(nameof(CmsContext.Languages));
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => x.IsDefault); // ISSUE: https://github.com/Logitar/CMS/issues/10
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => x.LocaleNormalized).IsUnique();

    builder.Property(x => x.Locale).HasMaxLength(LocaleUnit.MaximumLength);
    builder.Property(x => x.LocaleNormalized).HasMaxLength(LocaleUnit.MaximumLength);
  }
}
