using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ERPSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string baseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            //baseUrl = configuration["BaseUrl"];
        }

        public IActionResult Index()
        {
            // ViewBag.BaseUrl = baseUrl; // Assign the base URL to ViewBag
            var a = Directory.GetCurrentDirectory();

            return View();
        }

        public IActionResult Privacy()
        {
            //var co = new TextBox();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
