using AspNetCore.Models;
using AspNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDateService _dateService;
        private readonly TimeService _timeService;
        private readonly DateTimeService _dateTimeService;

        public HomeController(
            ILogger<HomeController> logger, 
            IDateService dateService,
            TimeService timeService,
            DateTimeService dateTimeService)
        {
            _logger = logger;
            _dateService = dateService;
            _timeService = timeService;
            _dateTimeService = dateTimeService;
        }

        public IActionResult Index()
        {
            // HomeWork1
            string[] arr = { "value01", "value02", "value03" };

            return View(arr);
        }

        public IActionResult Intro()
        {
            return View();
        }

        public IActionResult Razor()
        {
            return View();
        }

        public ViewResult Services()
        {
            ViewData["Date"] = _dateService.GetDate();
            ViewData["Time"] = _timeService.GetTime();
            ViewData["DateTime"] = _dateTimeService.GetNow();

            ViewData["HCHash"] = this.GetHashCode();
            ViewData["DSHash"] = _dateService.GetHashCode();
            ViewData["TSHash"] = _timeService.GetHashCode();
            ViewData["DTSHash"] = _dateTimeService.GetHashCode();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}