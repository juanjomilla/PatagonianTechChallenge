using Microsoft.AspNetCore.Mvc;

namespace PatagonianChallengeAPI.Controllers
{
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public ActionResult<string> Index()
        {
            return Ok("The .NET Core API is running!");
        }
    }
}
