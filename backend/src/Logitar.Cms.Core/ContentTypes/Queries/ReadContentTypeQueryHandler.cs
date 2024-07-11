using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

internal class ReadContentTypeQueryHandler : IRequestHandler<ReadContentTypeQuery, CmsContentType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public ReadContentTypeQueryHandler(IContentTypeQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<CmsContentType?> Handle(ReadContentTypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, CmsContentType> contentTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      CmsContentType? contentType = await _contentTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      CmsContentType? contentType = await _contentTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (contentTypes.Count > 1)
    {
      throw TooManyResultsException<CmsContentType>.ExpectedSingle(contentTypes.Count);
    }

    return contentTypes.Values.SingleOrDefault();
  }
}
