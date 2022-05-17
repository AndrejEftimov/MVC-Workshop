using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Workshop.Areas.Identity.Data;
using MVC_Workshop.Models;
using System.Diagnostics;

namespace MVC_Workshop.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<MVCWorkshopUser> userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<MVCWorkshopUser> UserManager)
        {
            _logger = logger;
            userManager = UserManager;
        }

        public async Task<IActionResult> Index()
        {
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            if (currUser != null)
            {
                if (currUser.Role == "Student")
                    return RedirectToAction("StudentIndex", "Enrollments", new { studentId = currUser.StudentId });

                else if (currUser.Role == "Teacher")
                    return RedirectToAction("TeacherIndex", "Courses", new { teacherId = currUser.TeacherId });
            }

            // if currUser is null or if role is "Admin"
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