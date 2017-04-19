using Microsoft.AspNetCore.Mvc;

namespace Mosaico.NetCore.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("editor")]
        public IActionResult Editor()
        {
            return View();
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}