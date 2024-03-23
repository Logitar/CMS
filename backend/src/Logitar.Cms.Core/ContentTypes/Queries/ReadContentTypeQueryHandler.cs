using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

internal class ReadContentTypeQueryHandler : IRequestHandler<ReadContentTypeQuery, ContentType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public ReadContentTypeQueryHandler(IContentTypeQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<ContentType?> Handle(ReadContentTypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentType> contentTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ContentType? contentType = await _contentTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      ContentType? contentType = await _contentTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (contentTypes.Count > 1)
    {
      throw TooManyResultsException<ContentType>.ExpectedSingle(contentTypes.Count);
    }

    return contentTypes.SingleOrDefault().Value;
  }
}
