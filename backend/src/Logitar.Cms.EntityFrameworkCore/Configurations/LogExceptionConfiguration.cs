using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class LogExceptionConfiguration : IEntityTypeConfiguration<LogExceptionEntity>
{
  public void Configure(EntityTypeBuilder<LogExceptionEntity> builder)
  {
    builder.ToTable(CmsDb.LogExceptions.Table.Table ?? string.Empty, CmsDb.LogExceptions.Table.Schema);
    builder.HasKey(x => x.LogExceptionId);

    builder.HasIndex(x => x.Type);

    builder.Ignore(x => x.Data);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DataSerialized).HasColumnName(nameof(LogExceptionEntity.Data));

    builder.HasOne(x => x.Log).WithMany(x => x.Exceptions).OnDelete(DeleteBehavior.Cascade);
  }
}
