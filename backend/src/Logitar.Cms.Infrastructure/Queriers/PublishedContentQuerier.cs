using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class PublishedContentQuerier : IPublishedContentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<PublishedContentEntity> _publishedContents;

  public PublishedContentQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _publishedContents = context.PublishedContents;
  }

  public async Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] locales = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentId == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapAsync(locales, cancellationToken)).Single();
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] locales = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentUid == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapAsync(locales, cancellationToken)).Single();
  }

  private async Task<IReadOnlyCollection<PublishedContent>> MapAsync(IEnumerable<PublishedContentEntity> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    // TODO(fpion): do we want to include Languages?
    // TODO(fpion): do we want to include ContentTypes, FieldDefinitions and FieldTypes?
    throw new NotImplementedException();
  }
}
