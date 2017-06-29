using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Mosaico.NetCore.Controllers
{
    [Route("templates")]
    public class TemplateController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public TemplateController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        // NOTE: Ignore {name1}. It's actually just the same as {name}, but MVC routing won't allow us to use {name} twice
        [Route("{name}/template-{name1}")]
        public IActionResult Index(string name)
        {
            string filePath = Path.Combine(hostingEnvironment.WebRootPath, string.Format("templates/{0}/template-{0}.html", name));
            string content = System.IO.File.ReadAllText(filePath);
            return Content(content);
        }
    }
}