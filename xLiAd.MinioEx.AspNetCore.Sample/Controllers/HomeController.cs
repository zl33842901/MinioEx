using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xLiAd.MinioEx.AspNetCore.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IMinioEx minioEx;
        public HomeController(IMinioEx minioEx)
        {
            this.minioEx = minioEx;
        }
        [HttpPost, Route("[action]")]
        public async Task<IActionResult> Upload()
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var result = await minioEx.UploadAsync(stream, System.IO.Path.GetExtension(file.FileName), file.ContentType);
                //生成个缩略图链接
                if(result.Length > 1)
                {
                    var thumb = result[1].Insert(result[1].IndexOf('/', 9) + 1, "200x300");
                    result = result.ToList().Union(new string[] { thumb }).ToArray();
                }
                return Json(new { suc = true, path = result });
            }
            else
                return Json(new { suc = false, msg = "没有上传数据" });
        }
    }
}
