#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Data;
using MVC_Workshop.Models;
using MVC_Workshop.ViewModels;

namespace MVC_Workshop.Controllers
{
    [Authorize]
    public class TeachersController : Controller
    {
        private readonly MVCWorkshopContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TeachersController(MVCWorkshopContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Teachers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string firstname, string lastname, string degree, string academicRank)
        {
            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();

            if (!String.IsNullOrEmpty(firstname))
            {
                teachers = teachers.Where(t => t.FirstName.Contains(firstname));
            }

            if (!String.IsNullOrEmpty(lastname))
            {
                teachers = teachers.Where(t => t.LastName.Contains(lastname));
            }

            if (!String.IsNullOrEmpty(degree))
            {
                teachers = teachers.Where(t => t.Degree.Contains(degree));
            }

            if (!String.IsNullOrEmpty(academicRank))
            {
                teachers = teachers.Where(t => t.AcademicRank.Contains(academicRank));
            }

            teachers = teachers.Include(t => t.CoursesFirst).Include(t => t.CoursesSecond);

            return View(teachers.ToList());
        }

        // GET: Teachers/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.Include(t => t.CoursesFirst).Include(t => t.CoursesSecond)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TeacherViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ProfileImage != null)
                {
                    string uniqueFileName = UploadedFile(viewModel);
                    viewModel.Teacher.ProfilePicture = uniqueFileName;
                }

                else
                {
                    viewModel.Teacher.ProfilePicture = "_default.png";
                }

                _context.Add(viewModel.Teacher);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            TeacherViewModel viewModel = new TeacherViewModel
            {
                Teacher = teacher,
                ProfileImage = null
            };

            return View(viewModel);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, TeacherViewModel viewModel)
        {
            if (id != viewModel.Teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.ProfileImage != null)
                    {
                        string uniqueFileName = UploadedFile(viewModel);
                        viewModel.Teacher.ProfilePicture = uniqueFileName;
                    }

                    _context.Update(viewModel.Teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(viewModel.Teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }

        // NOT USED IN APPLICATION
        public async Task<IActionResult> TeacherLogin()
        {
            return View(_context.Teacher);
        }

        private string UploadedFile(TeacherViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
