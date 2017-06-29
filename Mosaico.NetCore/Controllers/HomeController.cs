using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        [HttpPost]
        [Route("mosaico/dl")]
        public IActionResult Download()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("mosaico/upload")]
        public IActionResult GetUploads()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("mosaico/upload")]
        public IActionResult Upload(IEnumerable<IFormFile> files)
        {
            throw new NotImplementedException();
        }

        [Route("mosaico/img")]
        public async Task<IActionResult> Image(string src, string method, string @params)
        {
            var split = @params.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            switch (method)
            {
                case "cover": //TODO
                case "resize": //TODO
                case "placeholder":
                default:
                    {
                        string width = split[0];
                        string height = split[1];

                        using (var client = new HttpClient())
                        {
                            string url = string.Format("http://via.placeholder.com/{0}x{1}", width, height);
                            byte[] bytes = await client.GetByteArrayAsync(new Uri(url));
                            return File(bytes, "image/jpg");
                        }
                    }
            }
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}