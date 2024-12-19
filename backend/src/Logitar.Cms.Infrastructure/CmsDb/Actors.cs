using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class Actors
{
  public static readonly TableId Table = new(nameof(CmsContext.Actors));

  public static readonly ColumnId ActorId = new(nameof(ActorEntity.ActorId), Table);
  public static readonly ColumnId DisplayName = new(nameof(ActorEntity.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(ActorEntity.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(ActorEntity.Id), Table);
  public static readonly ColumnId IdHash = new(nameof(ActorEntity.IdHash), Table);
  public static readonly ColumnId IsDeleted = new(nameof(ActorEntity.IsDeleted), Table);
  public static readonly ColumnId PictureUrl = new(nameof(ActorEntity.PictureUrl), Table);
  public static readonly ColumnId Type = new(nameof(ActorEntity.Type), Table);
}
