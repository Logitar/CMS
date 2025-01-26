using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Web.Models.Content;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/published/contents")]
public class PublishedContentController : ControllerBase
{
  private readonly IMediator _mediator;

  public PublishedContentController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PublishedContent>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishedContent? content = await _mediator.Send(new ReadPublishedContentQuery(ContentId: null, ContentUid: id, Key: null), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet("types/{contentType}/unique-name:{uniqueName}")]
  public async Task<ActionResult<PublishedContent>> ReadAsync(string contentType, string uniqueName, string? language, CancellationToken cancellationToken)
  {
    PublishedContentKey key = new(contentType, language, uniqueName);
    PublishedContent? content = await _mediator.Send(new ReadPublishedContentQuery(ContentId: null, ContentUid: null, key), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PublishedContentLocale>>> SearchAsync([FromQuery] SearchPublishedContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<PublishedContentLocale> contents = await _mediator.Send(new SearchPublishedContentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contents);
  }
}
