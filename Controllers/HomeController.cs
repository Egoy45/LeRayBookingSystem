using System.Diagnostics;
using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeRayBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Title = "Welcome to Le'Ray Booking System",
                Message = "This is your homepage powered by ASP.NET Core MVC."
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            var model = new PrivacyViewModel
            {
                Title = "Privacy Policy",
                Details = "Your privacy is important to us. This page explains how we handle your data."
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    // Extra ViewModels for Home
    public class HomeViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class PrivacyViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
