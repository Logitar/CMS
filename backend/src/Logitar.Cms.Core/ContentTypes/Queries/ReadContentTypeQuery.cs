using Logitar.Cms.Contracts.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Queries;

public record ReadContentTypeQuery(Guid? Id, string? UniqueName) : Activity, IRequest<ContentTypeModel?>;

internal class ReadContentTypeQueryHandler : IRequestHandler<ReadContentTypeQuery, ContentTypeModel?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;

  public ReadContentTypeQueryHandler(IContentTypeQuerier contentTypeQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
  }

  public async Task<ContentTypeModel?> Handle(ReadContentTypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentTypeModel> contentTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ContentTypeModel? contentType = await _contentTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      ContentTypeModel? contentType = await _contentTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (contentType != null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (contentTypes.Count > 1)
    {
      throw TooManyResultsException<ContentTypeModel>.ExpectedOne(contentTypes.Count);
    }

    return contentTypes.Values.SingleOrDefault();
  }
}
