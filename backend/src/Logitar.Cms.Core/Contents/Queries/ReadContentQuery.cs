using Logitar.Cms.Core.Contents.Models;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

public record ReadContentQuery(Guid Id) : IRequest<ContentModel?>;

internal class ReadContentQueryHandler : IRequestHandler<ReadContentQuery, ContentModel?>
{
  private readonly IContentQuerier _contentQuerier;

  public ReadContentQueryHandler(IContentQuerier contentQuerier)
  {
    _contentQuerier = contentQuerier;
  }

  public async Task<ContentModel?> Handle(ReadContentQuery query, CancellationToken cancellationToken)
  {
    return await _contentQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
