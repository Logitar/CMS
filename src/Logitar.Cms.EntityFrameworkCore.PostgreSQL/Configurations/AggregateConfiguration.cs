using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Configurations;

internal abstract class AggregateConfiguration<T> where T : AggregateEntity
{
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasIndex(x => x.AggregateId).IsUnique();
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.AggregateId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CreatedById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UpdatedById).HasMaxLength(byte.MaxValue);
  }
}
