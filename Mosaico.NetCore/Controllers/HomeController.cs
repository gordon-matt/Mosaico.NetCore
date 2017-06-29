using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Mosaico.NetCore.Helpers;

namespace Mosaico.NetCore.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

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
        public async Task<IActionResult> Download(string action, string filename, string html)
        {
            switch (action)
            {
                case "download":
                    {
                        var bytes = Encoding.UTF8.GetBytes(html);
                        return File(bytes, "text/html", filename);
                    }
                case "email":
                default: throw new ArgumentException("Unsuported action type: " + action);
            }
        }

        [HttpGet]
        [Route("mosaico/upload")]
        public IActionResult GetUploads()
        {
            string path = Path.Combine(hostingEnvironment.WebRootPath, "Media/Uploads");

            var files = (new DirectoryInfo(path)).GetFiles().Select(x => new MosaicoFileInfo
            {
                Name = x.Name,
                Size = x.Length,
                Type = MimeMapping.GetMimeMapping(x.Name),
                Url = string.Concat("/Media/Uploads/", x.Name), // TODO: Use absolute URLs, so that images show when the HTML is generated and emailed or downloaded
                ThumbnailUrl = string.Concat("/Media/Thumbs/", x.Name),
                DeleteUrl = string.Concat("/mosaico/delete/", x.Name),
                DeleteType = "DELETE"
            });

            return Ok(new { files = files });
        }

        [HttpPost]
        [Route("mosaico/upload")]
        public async Task<IActionResult> Upload()
        {
            var files = Request.Form.Files;
            var returnList = new List<MosaicoFileInfo>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Media/Uploads", file.FileName);
                    string thumbPath = Path.Combine(hostingEnvironment.WebRootPath, "Media/Thumbs", file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        var image = Bitmap.FromStream(stream);
                        var thumbnail = ImageHelper.ResizeImage(image, 120, 90);
                        thumbnail.Save(thumbPath);
                    }

                    returnList.Add(new MosaicoFileInfo
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Type = MimeMapping.GetMimeMapping(file.FileName),
                        Url = string.Concat("/Media/Uploads/", file.FileName), // TODO: Use absolute URLs, so that images show when the HTML is generated and emailed or downloaded
                        ThumbnailUrl = string.Concat("/Media/Thumbs/", file.FileName),
                        DeleteUrl = string.Concat("/mosaico/delete/", file.FileName),
                        DeleteType = "DELETE"
                    });
                }
            }

            return Ok(new { files = returnList });
        }

        [HttpDelete]
        [Route("mosaico/delete/{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Media/Uploads", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Ok();
        }

        [Route("mosaico/img")]
        public async Task<IActionResult> Image(string src, string method, string @params)
        {
            var split = @params.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            switch (method)
            {
                case "resize":
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, src.TrimStart(new[] { '/' }));
                        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

                        Bitmap resizedImage;
                        using (var stream = new MemoryStream(bytes))
                        {
                            var image = Bitmap.FromStream(stream);

                            int width = split[0] == "null" ? image.Width : int.Parse(split[0]);
                            int height = split[1] == "null" ? image.Height : int.Parse(split[1]);

                            resizedImage = ImageHelper.ResizeImage(image, width, height);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            resizedImage.Save(memoryStream, ImageFormat.Jpeg);
                            byte[] newBytes = memoryStream.ToArray();
                            return File(newBytes, "image/jpg");
                        }
                    }
                case "cover": //TODO
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

        public class MosaicoFileInfo
        {
            public string Name { get; set; }

            public long Size { get; set; }

            public string Type { get; set; }

            public string Url { get; set; }

            public string ThumbnailUrl { get; set; }

            public string DeleteUrl { get; set; }

            public string DeleteType { get; set; }
        }
    }
}