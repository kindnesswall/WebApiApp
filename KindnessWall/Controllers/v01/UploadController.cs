using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using KindnessWall.Helper;

namespace KindnessWall.Controllers.v01
{
    [Authorize]
    public class UploadController : BaseApiController
    {
        [HttpPost]
        [Route("api/v01/Upload")]
        public async Task<IHttpActionResult> Upload()
        {
            var currentUser = User.Identity;
            try
            {
                const string uploadPath = "/Upload/";

                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/{uploadPath}")))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/{uploadPath}"));

                var file = await Request.Content.ReadAsByteArrayAsync();
                var fileName = Request.Headers.GetValues("fileName").FirstOrDefault();

                var fileExtension = Path.GetExtension(fileName);
                var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp" };
                if (!validExtensions.Contains(fileExtension)) return BadRequest("unsupported file format");

                var name = $"{uploadPath}{Guid.NewGuid()}{fileExtension}";

                var savePath = HttpContext.Current.Server.MapPath($"~/{name}");

                var fu = new FileUpload();
                fu.CropAndResizeImage(file, savePath, new Size { Width = 400, Height = 400 }, ImageFormat.Png, true);

                var fullAddress = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + name;
                return Created(fullAddress, new { imageSrc = fullAddress });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.InnerException?.Message);
            }
        }

    }
}
