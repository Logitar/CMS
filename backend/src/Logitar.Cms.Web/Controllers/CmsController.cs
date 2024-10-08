using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("cms")]
public class CmsController : Controller
{
  [HttpGet("{**anything}")]
  public ActionResult Index() => View();
}
