using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Contents.Commands;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Web.Models.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/contents")]
public class ContentController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public ContentController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<ContentItem>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken)
  {
    ContentItem content = await _pipeline.ExecuteAsync(new CreateContentCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/contents/{id}", [new KeyValuePair<string, string>("id", content.Id.ToString())]);

    return Created(location, content);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentItem>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentItem? content = await _pipeline.ExecuteAsync(new ReadContentQuery(id), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPut("{contentId}/invariant")]
  public async Task<ActionResult<ContentItem>> SaveInvariantAsync(Guid contentId, [FromBody] SaveContentLocalePayload payload, CancellationToken cancellationToken)
  {
    ContentItem? content = await _pipeline.ExecuteAsync(new SaveContentLocaleCommand(contentId, LanguageId: null, payload), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpPut("{contentId}/locales/{languageId}")]
  public async Task<ActionResult<ContentItem>> SaveLocaleAsync(Guid contentId, Guid languageId, [FromBody] SaveContentLocalePayload payload, CancellationToken cancellationToken)
  {
    ContentItem? content = await _pipeline.ExecuteAsync(new SaveContentLocaleCommand(contentId, languageId, payload), cancellationToken);
    return content == null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentLocale>>> SearchAsync([FromQuery] SearchContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentLocale> contents = await _pipeline.ExecuteAsync(new SearchContentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contents);
  }
}
