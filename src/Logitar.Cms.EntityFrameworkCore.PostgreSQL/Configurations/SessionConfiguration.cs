using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Configurations;

internal class SessionConfiguration : AggregateConfiguration<SessionEntity>, IEntityTypeConfiguration<SessionEntity>
{
  public override void Configure(EntityTypeBuilder<SessionEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.SessionId);

    builder.HasOne(x => x.User).WithMany(x => x.Sessions).OnDelete(DeleteBehavior.Restrict);

    builder.Property(x => x.Secret).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.SignedOutById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.IpAddress).HasMaxLength(byte.MaxValue);
  }
}
