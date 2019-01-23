using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using System.Diagnostics;

namespace WebApplication.Controllers
{
    public class ErrorController : Controller
    {

        public ErrorController()
        {
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
