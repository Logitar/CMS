using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class DateTimeFieldIndexConfiguration : FieldIndexConfiguration<DateTimeFieldIndexEntity, DateTime>, IEntityTypeConfiguration<DateTimeFieldIndexEntity>
{
  public override void Configure(EntityTypeBuilder<DateTimeFieldIndexEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.DateTimeFieldIndex.Table.Table ?? string.Empty, CmsDb.DateTimeFieldIndex.Table.Schema);

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.DateTimeFieldIndex.DateTimeFieldIndexId.Name);
  }
}
