using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Mosaico.NetCore.Configuration;
using Mosaico.NetCore.Extensions;
using Mosaico.NetCore.Helpers;

namespace Mosaico.NetCore.Controllers
{
    [Route("mosaico")]
    public class MosaicoController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptions<SmtpOptions> smtpOptions;

        public MosaicoController(
            IHostingEnvironment hostingEnvironment,
            IOptions<SmtpOptions> smtpOptions)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.smtpOptions = smtpOptions;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Title = "Free responsive email template editor | Mosaico.io";
            return View();
        }

        [Route("editor")]
        public IActionResult Editor()
        {
            ViewBag.Title = "Mosaico Editor";
            return View();
        }

        [HttpPost]
        [Route("dl")]
        public async Task<IActionResult> Download(
            string action,
            string filename,
            string rcpt,
            string subject,
            string html)
        {
            switch (action)
            {
                case "download":
                    {
                        var bytes = Encoding.UTF8.GetBytes(html);
                        return File(bytes, "text/html", filename);
                    }
                case "email":
                    {
                        var message = new MimeMessage
                        {
                            Subject = subject,
                            Body = new TextPart(TextFormat.Html) { Text = html }
                        };

                        message.From.Add(new MailboxAddress(smtpOptions.Value.FromName, smtpOptions.Value.FromEmail));
                        message.To.Add(new MailboxAddress(rcpt));

                        using (var smtpClient = new SmtpClient())
                        {
                            await smtpClient.ConnectAsync(smtpOptions.Value.Host, smtpOptions.Value.Port, false);
                            await smtpClient.AuthenticateAsync(smtpOptions.Value.UserName, smtpOptions.Value.Password);
                            await smtpClient.SendAsync(message);
                            await smtpClient.DisconnectAsync(true);
                        }

                        return Ok();
                    }
                default: throw new ArgumentException("Unsuported action type: " + action);
            }
        }

        [HttpGet]
        [Route("upload")]
        public IActionResult GetUploads()
        {
            string path = Path.Combine(hostingEnvironment.WebRootPath, "Media/Uploads");

            var files = (new DirectoryInfo(path)).GetFiles().Select(x => new MosaicoFileInfo
            {
                Name = x.Name,
                Size = x.Length,
                Type = MimeMapping.GetMimeMapping(x.Name),
                Url = Url.AbsoluteContent(string.Concat("/Media/Uploads/", x.Name)),
                ThumbnailUrl = Url.AbsoluteContent(string.Concat("/Media/Thumbs/", x.Name)),
                DeleteUrl = string.Concat("/mosaico/delete/", x.Name),
                DeleteType = "DELETE"
            });

            return Ok(new { files = files });
        }

        [HttpPost]
        [Route("upload")]
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
                        var image = System.Drawing.Image.FromStream(stream);
                        var thumbnail = ImageHelper.Resize(image, 120, 90);
                        thumbnail.Save(thumbPath);
                    }

                    returnList.Add(new MosaicoFileInfo
                    {
                        Name = file.FileName,
                        Size = file.Length,
                        Type = MimeMapping.GetMimeMapping(file.FileName),
                        Url = Url.AbsoluteContent(string.Concat("/Media/Uploads/", file.FileName)),
                        ThumbnailUrl = Url.AbsoluteContent(string.Concat("/Media/Thumbs/", file.FileName)),
                        DeleteUrl = string.Concat("/mosaico/delete/", file.FileName),
                        DeleteType = "DELETE"
                    });
                }
            }

            return Ok(new { files = returnList });
        }

        [HttpDelete]
        [Route("delete/{fileName}")]
        public IActionResult Delete(string fileName)
        {
            string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Media/Uploads", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Ok();
        }

        [Route("img")]
        public async Task<IActionResult> Image(string src, string method, string @params)
        {
            var split = @params.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            switch (method)
            {
                case "cover":
                case "resize":
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Media\\Uploads\\", src.RightOfLastIndexOf('/'));
                        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

                        Image result;
                        using (var stream = new MemoryStream(bytes))
                        {
                            var image = Bitmap.FromStream(stream);

                            int? destinationWidth = split[0] == "null" ? null : (int?)int.Parse(split[0]);
                            int? destinationHeight = split[1] == "null" ? null : (int?)int.Parse(split[1]);

                            if (destinationWidth.HasValue && destinationHeight.HasValue)
                            {
                                if (method == "cover")
                                {
                                    result = ImageHelper.Crop(image, destinationWidth.Value, destinationHeight.Value, AnchorPosition.Center);
                                }
                                else
                                {
                                    result = ImageHelper.Resize(image, destinationWidth.Value, destinationHeight.Value);
                                }
                            }
                            else if (destinationWidth.HasValue)
                            {
                                var newHeight = destinationWidth.Value * image.Height / image.Width;
                                result = ImageHelper.Resize(image, destinationWidth.Value, newHeight);
                            }
                            else if (destinationHeight.HasValue)
                            {
                                var newWidth = destinationHeight.Value * image.Width / image.Height;
                                result = ImageHelper.Resize(image, newWidth, destinationHeight.Value);
                            }
                            else
                            {
                                throw new ArgumentException("A destination width and/or height must be specified.");
                            }
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            result.Save(memoryStream, ImageFormat.Jpeg);
                            byte[] newBytes = memoryStream.ToArray();
                            return File(newBytes, "image/jpg");
                        }
                    }
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