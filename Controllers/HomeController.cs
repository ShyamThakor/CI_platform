using CI_platform.Datamodel.DataModels;
using CI_platform.Models;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using System.Diagnostics;
using System.Net.Mail;


namespace CI_platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CiPlatformContext _db;

        public HomeController(ILogger<HomeController> logger, CiPlatformContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        public IActionResult Login(User obj)
        {
            if (obj != null)
            {
                var status = _db.Users.Where(m => m.Email == obj.Email && m.Password == obj.Password).Count();
                if (status > 0)
                {
                    return RedirectToAction("Landing", "Landing");
                }
                else
                {
                    TempData["Message"] = "Enter Valid Username and password";
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult forgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult forgotPassword(String Email)
        {
            if (Email != null)
            {
                var status = _db.Users.Where(m => m.Email == Email).Count();

                if (status != null)
                {
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringchars = new char[16];
                    var random = new Random();
                    for (int i = 0; i < stringchars.Length; i++)
                    {
                        stringchars[i] = chars[random.Next(chars.Length)];

                    }
                    var finalString = new String(stringchars);

                    Datamodel.DataModels.PasswordReset entry = new Datamodel.DataModels.PasswordReset();
                    
                    entry.Email = Email;
                    entry.Token = finalString;
                    _db.PasswordResets.Add(entry);
                    _db.SaveChanges();
                    HttpContext.Session.SetString("token_session", finalString);

                    var mailbody = "<h1>Click link to reset password</h1><br><h2><a href='" + "https://localhost:7296/Home/ResetPassword?token=" + finalString + "'>Reset Password</a></h2>";


                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("bvmalumini1020@gmail.com"));
                    email.To.Add(MailboxAddress.Parse(Email));
                    email.Subject = "Reset Your Password";
                    email.Body = new TextPart(TextFormat.Html) { Text = mailbody };

                    // send email
                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("bvmalumini1020@gmail.com", "rpratpmgdxtspmdl");
                    smtp.Send(email);
                    smtp.Disconnect(true);


                    TempData["Error"] = "Check your email to reset password";
                    return RedirectToAction("Index", "Home");

                }
            }

            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
       public IActionResult Resetpassword(User model)
        {
            /*string url = HttpContext.Request.GetDisplayUrl();*/
            string token = HttpContext.Session.GetString("token_session");
            var validtoken = _db.PasswordResets.Where(m => m.Token == token).FirstOrDefault();

            if (validtoken != null)
            {
                var user = _db.Users.FirstOrDefault(x => x.Email == validtoken.Email);
                user.Password = model.Password;
                _db.Users.Update(user);
                _db.SaveChanges();
                TempData["Error"] = "Password changed";
                HttpContext.Session.Remove("token_session");
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Forgotpassword", "Home");
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
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                Password = obj.Password,
                CityId = 1,
                CountryId = 1

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