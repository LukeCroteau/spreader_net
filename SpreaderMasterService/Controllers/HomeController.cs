using Microsoft.AspNetCore.Mvc;

namespace SpreaderMasterService.Controllers
{
    public class HomeController : Controller
    {
        [Route("controllertest")]
        public IActionResult Index()
        {
            return Ok("Unimplemented Controller Test - Seriously Though");
        }
    }
}
