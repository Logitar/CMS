using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.ContentTypes;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents;

public class ContentAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings();

  public new ContentId Id => new(base.Id);

  private ContentTypeId? _contentTypeId = null;
  public ContentTypeId ContentTypeId => _contentTypeId ?? throw new InvalidOperationException($"The {nameof(ContentTypeId)} has not been initialized yet.");

  private readonly List<ContentLocaleUnit> _locales = [];
  public IReadOnlyCollection<ContentLocaleUnit> Locales => _locales.AsReadOnly();

  public ContentAggregate(AggregateId id) : base(id)
  {
  }

  public ContentAggregate(ContentTypeAggregate contentType, UniqueNameUnit uniqueName, ActorId actorId = default, ContentId? id = null)
    : base((id ?? ContentId.NewId()).AggregateId)
  {
    Raise(new ContentCreatedEvent(contentType.Id, uniqueName, actorId));
  }
  protected virtual void Apply(ContentCreatedEvent @event)
  {
    _contentTypeId = @event.ContentTypeId;

    _locales.Add(new ContentLocaleUnit(@event.UniqueName));
  }

  public override string ToString() => base.ToString(); // TODO(fpion): implement
}
