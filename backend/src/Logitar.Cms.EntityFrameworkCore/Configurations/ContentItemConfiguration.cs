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

    builder.ToTable(nameof(CmsContext.ContentItems));
    builder.HasKey(x => x.ContentItemId);

    builder.HasOne(x => x.ContentType).WithMany(x => x.ContentItems).OnDelete(DeleteBehavior.Restrict);
  }
}
