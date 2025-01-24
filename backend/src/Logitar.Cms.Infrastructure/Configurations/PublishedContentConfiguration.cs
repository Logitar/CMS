using Logitar.Cms.Infrastructure.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.Infrastructure.Configurations;

internal class PublishedContentConfiguration : AggregateConfiguration<PublishedContentEntity>, IEntityTypeConfiguration<PublishedContentEntity>
{
  public override void Configure(EntityTypeBuilder<PublishedContentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.PublishedContents.Table.Table ?? string.Empty, CmsDb.PublishedContents.Table.Schema);
    builder.HasKey(x => x.ContentLocaleId);

    // TODO(fpion): Indices

    // TODO(fpion): Properties

    builder.HasOne(x => x.ContentLocale).WithOne(x => x.PublishedContent)
      .HasPrincipalKey<ContentLocaleEntity>(x => x.ContentLocaleId)
      .HasForeignKey<PublishedContentEntity>(x => x.ContentLocaleId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
