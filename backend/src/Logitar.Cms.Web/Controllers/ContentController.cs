using Logitar.Cms.Core.Contents.Commands;
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
[Route("api/contents")]
public class ContentController : ControllerBase
{
  private readonly IMediator _mediator;

  public ContentController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<ContentModel>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken)
  {
    ContentModel content = await _mediator.Send(new CreateContentCommand(payload), cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/{content.Id}", UriKind.Absolute);
    return Created(location, content);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new ReadContentQuery(id), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPut("{contentId}")]
  public async Task<ActionResult<ContentModel>> SaveLocaleAsync(Guid contentId, Guid? languageId, [FromBody] SaveContentLocalePayload payload, long? version, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new SaveContentLocaleCommand(contentId, languageId, payload, version), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentLocaleModel>>> SearchAsync([FromQuery] SearchContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentLocaleModel> contents = await _mediator.Send(new SearchContentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contents);
  }

  [HttpPatch("{contentId}")]
  public async Task<ActionResult<ContentModel>> UpdateLocaleAsync(Guid contentId, Guid? languageId, [FromBody] UpdateContentLocalePayload payload, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new UpdateContentLocaleCommand(contentId, languageId, payload), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }
}
