using Logitar.Cms.Infrastructure.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.Infrastructure.Configurations;

internal class ContentConfiguration : AggregateConfiguration<ContentEntity>, IEntityTypeConfiguration<ContentEntity>
{
  public override void Configure(EntityTypeBuilder<ContentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.Contents.Table.Table ?? string.Empty, CmsDb.Contents.Table.Schema);
    builder.HasKey(x => x.ContentId);

    builder.HasIndex(x => x.Id).IsUnique();

    builder.HasOne(x => x.ContentType).WithMany(x => x.Contents)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
