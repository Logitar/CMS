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
    return await _contentQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
