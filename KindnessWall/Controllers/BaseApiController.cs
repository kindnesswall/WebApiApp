using KindnessWall.Models;
using System.Web.Http;

namespace KindnessWall.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly ApplicationDbContext Context;

        public BaseApiController()
        {
            Context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Context.Dispose();

            base.Dispose(disposing);
        }
    }
}