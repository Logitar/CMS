using Logitar.Cms.Contracts.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers.Api;

[ApiController]
[Route("cms/api/resources")]
public class ResourceApiController : ControllerBase
{
  private readonly IResourceService _resourceService;

  public ResourceApiController(IResourceService resourceService)
  {
    _resourceService = resourceService;
  }

  [HttpGet("locales")]
  public ActionResult<IEnumerable<Locale>> GetAsync() => Ok(_resourceService.GetLocales());
}
