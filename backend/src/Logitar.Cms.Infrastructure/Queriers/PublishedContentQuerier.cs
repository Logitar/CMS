using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class PublishedContentQuerier : IPublishedContentQuerier
{
  private readonly DbSet<PublishedContentEntity> _publishedContents;

  public PublishedContentQuerier(CmsContext context)
  {
    _publishedContents = context.PublishedContents;
  }

  public async Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] publishedContents = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentId == contentId)
      .ToArrayAsync(cancellationToken);

    if (publishedContents.Length < 1)
    {
      return null;
    }

    throw new NotImplementedException(); // TODO(fpion): map
  }
  public async Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken)
  {
    PublishedContentEntity[] publishedContents = await _publishedContents.AsNoTracking()
      .Where(x => x.ContentUid == contentId)
      .ToArrayAsync(cancellationToken);

    if (publishedContents.Length < 1)
    {
      return null;
    }

    throw new NotImplementedException(); // TODO(fpion): map
  }
}
