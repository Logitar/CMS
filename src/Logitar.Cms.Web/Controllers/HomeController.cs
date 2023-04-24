using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("cms")]
public class HomeController : Controller
{
  [HttpGet]
  public ActionResult Index() => View();
}
