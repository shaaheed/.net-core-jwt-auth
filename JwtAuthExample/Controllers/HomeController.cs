using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthExample.Controllers
{
    [Route("api/home")]
    public class HomeController : Controller
    {

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return Ok("Works");
        }

    }
}
