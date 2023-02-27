using CI_platform.Datamodel.DataModels;
using CI_platform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CI_platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CIPlatformContext _db;

        public HomeController(ILogger<HomeController> logger, CIPlatformContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult forgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(User obj)
        {
            var User_data = new User()
            {
                FirstName= obj.FirstName,
                LastName= obj.LastName,
                Email= obj.Email,   
                PhoneNumber= obj.PhoneNumber,   
                Password = obj.Password,
                CityId=1,
                CountryId=1 

            };
            _db.Users.Add(User_data);
            _db.SaveChanges();
            return RedirectToAction("Index");
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