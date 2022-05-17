using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Areas.Identity.Data;
using MVC_Workshop.Data;
using MVC_Workshop.Models;

namespace MVC_Workshop.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly MVCWorkshopContext _context;

        private UserManager<MVCWorkshopUser> userManager;
        private IPasswordHasher<MVCWorkshopUser> passwordHasher;
        private IPasswordValidator<MVCWorkshopUser> passwordValidator;
        private IUserValidator<MVCWorkshopUser> userValidator;

        public AccountsController(MVCWorkshopContext context, UserManager<MVCWorkshopUser> UserManager, IPasswordHasher<MVCWorkshopUser> PasswordHasher,
            IPasswordValidator<MVCWorkshopUser> PasswordValidator, IUserValidator<MVCWorkshopUser> UserValidator)
        {
            _context = context;
            userManager = UserManager;
            passwordHasher = PasswordHasher;
            passwordValidator = PasswordValidator;
            userValidator = UserValidator;
        }

        // GET: Accounts
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? email, string? role)
        {
            var users = userManager.Users;

            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(u => u.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role.Contains(role));
            }

            users = users.Include(u => u.Student).Include(u => u.Teacher).OrderBy(u => u.Email);

            return View(users.ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            List<string> roles = new List<string>();
            roles.Add("Student");
            roles.Add("Teacher");
            ViewData["RoleList"] = new SelectList(roles);

            MVCWorkshopUser user = new MVCWorkshopUser();

            return View(user);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MVCWorkshopUser user, string? password)
        {
            IdentityResult validPass = null;
            bool validEntry = true;

            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("", "Email is empty");
                validEntry = false;
            }

            else
            {
                if (!IsValidEmail(user.Email))
                {
                    ModelState.AddModelError("", "Email is invalid");
                    validEntry = false;
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Password is empty");
                validEntry = false;
            }

            else
            {
                validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                if (!validPass.Succeeded)
                {
                    ModelState.AddModelError("", "Password is invalid");
                    validEntry = false;
                }
            }

            if ((user.StudentId == null && user.TeacherId == null) || (user.StudentId != null && user.TeacherId != null))
            {
                ModelState.AddModelError("", "Select Student/Teacher");
                validEntry = false;
            }

            if (validEntry == true)
            {
                user.UserName = user.Email;
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, user.Role);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    string message = GetIdentityResultErrors(result);
                    ModelState.AddModelError("", message);
                }
            }

            if (user.Role == "Student")
            {
                ViewData["StudentList"] = new SelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullNameWithId");
            }
            else if (user.Role == "Teacher")
            {
                ViewData["TeacherList"] = new SelectList(_context.Teacher.OrderBy(s => s.FirstName), "Id", "FullName");
            }

            List<string> roles = new List<string>();
            roles.Add("Student");
            roles.Add("Teacher");
            ViewData["RoleList"] = new SelectList(roles);

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEdit(string userId)
        {
            MVCWorkshopUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEdit(string userId, string? email, string? password, string? phoneNumber)
        {
            IdentityResult validPass = null;
            bool validEntry = true;
            MVCWorkshopUser user = await userManager.FindByIdAsync(userId);

            user.Email = email;
            user.UserName = email;
            user.PhoneNumber = phoneNumber;

            if(user == null)
                return NotFound();

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is empty");
                validEntry = false;
            }

            else
            {
                if (!IsValidEmail(email))
                {
                    ModelState.AddModelError("", "Email is invalid");
                    validEntry = false;
                }
            }

            if (!string.IsNullOrEmpty(password))
            {
                validPass = await passwordValidator.ValidateAsync(userManager, user, password);

                if (validPass.Succeeded)
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);

                else
                {
                    ModelState.AddModelError("", "Password is invalid");
                    validEntry = false;
                }
            }

            if ((user.StudentId == null && user.TeacherId == null) || (user.StudentId != null && user.TeacherId != null))
            {
                ModelState.AddModelError("", "Select Student/Teacher");
                validEntry = false;
            }

            if (validEntry == true)
            {
                IdentityResult result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    string message = GetIdentityResultErrors(result);
                    ModelState.AddModelError("", message);
                }
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            MVCWorkshopUser user = await userManager.FindByIdAsync(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            MVCWorkshopUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                {
                    string message = GetIdentityResultErrors(result);
                    ModelState.AddModelError("", message);
                }
            }

            else
                ModelState.AddModelError("", "Invalid User");

            return RedirectToAction("Delete", new { userId = id });
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public string GetIdentityResultErrors(IdentityResult result)
        {
            var message = string.Join(", ", result.Errors.Select(x => "Code - " + x.Code + " Description - " + x.Description));
            return message;
        }
    }
}
