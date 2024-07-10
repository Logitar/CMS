using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class LogEventConfiguration : IEntityTypeConfiguration<LogEventEntity>
{
  public void Configure(EntityTypeBuilder<LogEventEntity> builder)
  {
    builder.ToTable(CmsDb.LogEvents.Table.Table ?? string.Empty, CmsDb.LogEvents.Table.Schema);
    builder.HasKey(x => x.EventId);

    builder.HasIndex(x => x.LogId);

    builder.Property(x => x.EventId).HasMaxLength(EventId.MaximumLength);

    builder.HasOne(x => x.Log).WithMany(x => x.Events).OnDelete(DeleteBehavior.Cascade);
  }
}
