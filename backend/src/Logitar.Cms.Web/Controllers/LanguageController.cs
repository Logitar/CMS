using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Languages.Commands;
using Logitar.Cms.Core.Languages.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/languages")]
public class LanguageController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public LanguageController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<Language>> CreateAsync([FromBody] CreateLanguagePayload payload, CancellationToken cancellationToken)
  {
    Language language = await _pipeline.ExecuteAsync(new CreateLanguageCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/languages/{id}", [new KeyValuePair<string, string>("id", language.Id.ToString())]);

    return Created(location, language);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Language>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(id, Locale: null, IsDefault: false), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<Language>> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, locale, IsDefault: false), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("default")]
  public async Task<ActionResult<Language>> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, Locale: null, IsDefault: true), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<Language>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new SetDefaultLanguageCommand(id), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }
}
