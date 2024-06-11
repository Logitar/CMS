using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

internal class ReadContentTypeQueryHandler : IRequestHandler<ReadContentTypeQuery, ContentsType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public ReadContentTypeQueryHandler(IContentTypeQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<ContentsType?> Handle(ReadContentTypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentsType> contentTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ContentsType? contentType = await _contentTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      ContentsType? contentType = await _contentTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (contentTypes.Count > 1)
    {
      throw TooManyResultsException<ContentsType>.ExpectedSingle(contentTypes.Count);
    }

    return contentTypes.SingleOrDefault().Value;
  }
}
