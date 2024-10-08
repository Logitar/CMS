using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Languages.Commands;
using Logitar.Cms.Core.Languages.Queries;
using Logitar.Cms.Web.Models.Languages;
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
  public async Task<ActionResult<LanguageModel>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceLanguageCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(id, Locale: null, IsDefault: false), cancellationToken);
    return GetActionResult(language);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, locale, IsDefault: false), cancellationToken);
    return GetActionResult(language);
  }

  [HttpGet("default")]
  public async Task<ActionResult<LanguageModel>> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, Locale: null, IsDefault: true), cancellationToken);
    return GetActionResult(language);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LanguageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceLanguageCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LanguageModel>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<LanguageModel> languages = await _pipeline.ExecuteAsync(new SearchLanguagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<LanguageModel>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new SetDefaultLanguageCommand(id), cancellationToken);
    return GetActionResult(language);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LanguageModel>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new UpdateLanguageCommand(id, payload), cancellationToken);
    return GetActionResult(language);
  }

  private ActionResult<LanguageModel> GetActionResult(CreateOrReplaceLanguageResult result)
  {
    return GetActionResult(result.Language, result.Created);
  }
  private ActionResult<LanguageModel> GetActionResult(LanguageModel? language, bool created = false)
  {
    if (language == null)
    {
      return NotFound();
    }
    else if (created)
    {
      Uri location = HttpContext.BuildLocation("api/languages/{id}", [new KeyValuePair<string, string>("id", language.Id.ToString())]);
      return Created(location, language);
    }

    return Ok(language);
  }
}
