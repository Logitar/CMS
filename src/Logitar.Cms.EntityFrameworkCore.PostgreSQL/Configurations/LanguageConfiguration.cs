using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Configurations;

internal class LanguageConfiguration : AggregateConfiguration<LanguageEntity>, IEntityTypeConfiguration<LanguageEntity>
{
  public override void Configure(EntityTypeBuilder<LanguageEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => x.Locale).IsUnique();
    builder.HasIndex(x => x.IsDefault).IsUnique().HasFilter($@"""{nameof(LanguageEntity.IsDefault)}"" = true");

    builder.Property(x => x.Locale).HasMaxLength(byte.MaxValue);
  }
}
