using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class UniqueFieldIndexConfiguration : FieldIndexConfiguration<UniqueFieldIndexEntity, string>, IEntityTypeConfiguration<UniqueFieldIndexEntity>
{
  public override void Configure(EntityTypeBuilder<UniqueFieldIndexEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.UniqueFieldIndex.Table.Table ?? string.Empty, CmsDb.UniqueFieldIndex.Table.Schema);

    builder.HasIndex(x => new { x.FieldDefinitionId, x.LanguageId, x.Value }).IsUnique();

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.UniqueFieldIndex.UniqueFieldIndexId.Name);
    builder.Property(x => x.Value).HasMaxLength(UniqueFieldIndexEntity.MaximumLength);
  }
}
