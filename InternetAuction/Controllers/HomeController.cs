using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InternetAuction.Models;
using Microsoft.AspNetCore.Http;

namespace InternetAuction.Controllers
{
    /// <summary>
    /// Контроллер домашней страницы.
    /// Содержит методы отображения главной страницы и страницы ошибок.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _db;
        /// <summary>
        /// Контроллер домашней страницы.
        /// Содержит методы отображения главной страницы и страницы ошибок.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, ApplicationContext db)
        {
            _db = db;
            _logger = logger;
            
        }
        
        public IActionResult Index()
        {
            return View(_db.Lots.Where(i => i.FinishDate >= DateTime.Now).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}