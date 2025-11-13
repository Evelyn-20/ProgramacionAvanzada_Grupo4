using Microsoft.AspNetCore.Mvc;

namespace Pasteleria.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/NotFound
        [Route("Error/NotFound")]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        // GET: Error/InternalServerError
        [Route("Error/InternalServerError")]
        public IActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View();
        }

        // GET: Error/{statusCode}
        [Route("Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return RedirectToAction(nameof(NotFound));
                case 500:
                    return RedirectToAction(nameof(InternalServerError));
                default:
                    return View();
            }
        }
    }
}
