using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Configurations;

internal class UserConfiguration : AggregateConfiguration<UserEntity>, IEntityTypeConfiguration<UserEntity>
{
  public override void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.UserId);

    builder.HasIndex(x => x.UsernameNormalized).IsUnique();

    builder.Property(x => x.Username).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UsernameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordChangedById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Password).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.DisabledById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailAddressNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailVerifiedById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.FirstName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.LastName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.FullName).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.Locale).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Picture).HasMaxLength(ushort.MaxValue);
  }
}
