using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace Backend.Controllers
{
    [Route("api/customtest")]
    public class CustomTestController : ApiController
    {
        // GET api/CustomTest
        public string Get()
        {
            return "Hello from custom controller!";
        }
    }
}
