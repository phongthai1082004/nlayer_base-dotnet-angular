using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
