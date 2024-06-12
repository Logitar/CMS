using Logitar.Cms.Contracts.Contents;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

internal class ReadContentQueryHandler : IRequestHandler<ReadContentQuery, ContentItem?>
{
  private readonly IContentQuerier _contentQuerier;

  public ReadContentQueryHandler(IContentQuerier contentQuerier)
  {
    _contentQuerier = contentQuerier;
  }

  public async Task<ContentItem?> Handle(ReadContentQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentItem> contents = new(capacity: 1);

    if (query.Id.HasValue)
    {
      ContentItem? content = await _contentQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (content != null)
      {
        contents[content.Id] = content;
      }
    }

    if (contents.Count > 1)
    {
      throw TooManyResultsException<ContentItem>.ExpectedSingle(contents.Count);
    }

    return contents.Values.SingleOrDefault();
  }
}
