using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class ContentItemConfiguration : AggregateConfiguration<ContentItemEntity>, IEntityTypeConfiguration<ContentItemEntity>
{
  public override void Configure(EntityTypeBuilder<ContentItemEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.ContentItems.Table.Table ?? string.Empty, CmsDb.ContentItems.Table.Schema);
    builder.HasKey(x => x.ContentItemId);

    builder.HasIndex(x => x.UniqueId).IsUnique();

    builder.HasOne(x => x.ContentType).WithMany(x => x.ContentItems)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
