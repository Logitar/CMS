using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class NumberFieldIndexConfiguration : FieldIndexConfiguration<NumberFieldIndexEntity, double>, IEntityTypeConfiguration<NumberFieldIndexEntity>
{
  public override void Configure(EntityTypeBuilder<NumberFieldIndexEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.NumberFieldIndex.Table.Table ?? string.Empty, CmsDb.NumberFieldIndex.Table.Schema);

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.NumberFieldIndex.NumberFieldIndexId.Name);
  }
}
