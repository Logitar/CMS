using Logitar.Cms.Core;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class ContentTypeConfiguration : AggregateConfiguration<ContentTypeEntity>, IEntityTypeConfiguration<ContentTypeEntity>
{
  public override void Configure(EntityTypeBuilder<ContentTypeEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.ContentTypes.Table.Table ?? string.Empty, CmsDb.ContentTypes.Table.Schema);
    builder.HasKey(x => x.ContentTypeId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.IsInvariant);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
  }
}
