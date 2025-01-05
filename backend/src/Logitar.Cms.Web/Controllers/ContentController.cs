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
  public async Task<ActionResult<ContentModel>> CreateAsync(Guid? languageId, [FromBody] CreateOrReplaceContentPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentResult result = await _mediator.Send(new CreateOrReplaceContentCommand(ContentId: null, languageId, payload, Version: null), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentModel? content = await _mediator.Send(new ReadContentQuery(id), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPut("{contentId}")]
  public async Task<ActionResult<ContentModel>> SaveLocaleAsync(Guid contentId, Guid? languageId, [FromBody] CreateOrReplaceContentPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentResult result = await _mediator.Send(new CreateOrReplaceContentCommand(contentId, languageId, payload, version), cancellationToken);
    return ToActionResult(result);
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

  private ActionResult<ContentModel> ToActionResult(CreateOrReplaceContentResult result)
  {
    if (result.Content == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/{result.Content.Id}", UriKind.Absolute);
      return Created(location, result.Content);
    }

    return Ok(result.Content);
  }
}
