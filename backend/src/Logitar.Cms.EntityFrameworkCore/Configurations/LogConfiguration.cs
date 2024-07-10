using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class LogConfiguration : IEntityTypeConfiguration<LogEntity>
{
  public void Configure(EntityTypeBuilder<LogEntity> builder)
  {
    builder.ToTable(CmsDb.Logs.Table.Table ?? string.Empty, CmsDb.Logs.Table.Schema);
    builder.HasKey(x => x.LogId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.CorrelationId);
    builder.HasIndex(x => x.OperationType);
    builder.HasIndex(x => x.OperationName);
    builder.HasIndex(x => x.ActivityType);
    builder.HasIndex(x => x.StatusCode);
    builder.HasIndex(x => x.IsCompleted);
    builder.HasIndex(x => x.Level);
    builder.HasIndex(x => x.HasErrors);
    builder.HasIndex(x => x.StartedOn);
    builder.HasIndex(x => x.EndedOn);
    builder.HasIndex(x => x.Duration);
    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.ActorId);
    builder.HasIndex(x => x.ApiKeyId);
    builder.HasIndex(x => x.UserId);
    builder.HasIndex(x => x.SessionId);

    builder.Property(x => x.CorrelationId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Method).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Destination).HasMaxLength(UrlUnit.MaximumLength);
    builder.Property(x => x.Source).HasMaxLength(UrlUnit.MaximumLength);
    builder.Property(x => x.OperationType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.OperationName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ActivityType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Level).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<LogLevel>());
    builder.Property(x => x.TenantId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.ActorId).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.ApiKeyId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.UserId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.SessionId).HasMaxLength(AggregateId.MaximumLength);
  }
}
