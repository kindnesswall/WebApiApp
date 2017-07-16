using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KindnessWall.Controllers.v01
{
    public class AppController : BaseApiController
    {
        [HttpGet]
        [Route("api/v01/GetAppInfo/{clientVersion}")]
        public IHttpActionResult GetAppInfo(string clientVersion)
        {
            var updateInfo = GetUpdateVersion(clientVersion);
            var smsCenter = ""; // sms center number

            return Ok(new
            {
                updateInfo,
                smsCenter
            });
        }

        private object GetUpdateVersion(string clientVersion)
        {
            //get last version from db
            var appVersion = Context.AppVersions.OrderByDescending(x => x.Id).FirstOrDefault();
            if (appVersion == null) return BadRequest();

            //get changes related to last version
            var changes = Context.AppVersionChanges.Where(x => x.AppVersionId == appVersion.Id).Select(x => x)
                .OrderBy(x => x.ViewOrder).Select(x => x.Description).ToList();

            //convert last version string to int array
            var lastVersionArray = appVersion.LastUpdateVersion.Split('.').Select(int.Parse).ToList();

            //convert client version string to int array
            var clientVersionArray = clientVersion.Split('.').Select(int.Parse).ToList();

            //get client version in db
            var clientVersionInDb = Context.AppVersions.FirstOrDefault(x => x.Version == clientVersion);
            if (clientVersionInDb == null)
            {
                return new
                {
                    version = appVersion.Version,
                    apk_url = appVersion.ApkUrl,
                    force_update = "true",
                    changes = changes
                };
            }

            var needUpdate = lastVersionArray.Where((t, i) => t > clientVersionArray[i]).Any();

            if (!needUpdate) needUpdate = appVersion.LastUpdateVersion == clientVersion;


            return new
            {
                version = appVersion.Version,
                apk_url = appVersion.ApkUrl,
                force_update = needUpdate ? "true" : "false",
                changes = changes
            };
        }
    }
}