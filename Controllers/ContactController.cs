using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
