#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Areas.Identity.Data;
using MVC_Workshop.Data;
using MVC_Workshop.Models;
using MVC_Workshop.ViewModels;

namespace MVC_Workshop.Controllers
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly MVCWorkshopContext _context;
        private UserManager<MVCWorkshopUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public EnrollmentsController(MVCWorkshopContext context, UserManager<MVCWorkshopUser> UserManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            userManager = UserManager;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Enrollments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var enrollments = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);

            return View(await enrollments.ToListAsync());
        }

        // GET: Enrollments/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int courseId)
        {
            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", courseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId");

            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminarUrl,ProjectUrl,ExamPoints,SeminarPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);

            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);

            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminarUrl,ProjectUrl,ExamPoints,SeminarPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
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

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);

            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);

            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherCourseIndex(int courseId, int teacherId, int? year)
        {
            Teacher teacher = _context.Teacher.FirstOrDefault(t => t.Id == teacherId);
            Course course = _context.Course.FirstOrDefault(c => c.Id == courseId);

            if (teacher == null || course == null)
                return NotFound();

            // get logged in user
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            // check if logged in user has access
            if ((teacher.Id != currUser.TeacherId) || (course.FirstTeacherId != teacherId && course.SecondTeacherId != teacherId))
                return LocalRedirect("/Identity/Account/AccessDenied");

            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();

            enrollments = enrollments.Where(e => e.CourseId == courseId);

            if (year != null)
            {
                enrollments = enrollments.Where(e => e.Year == year);
            }

            enrollments = enrollments.Include(e => e.Student).Include(e => e.Course);

            ViewData["teacherId"] = teacherId;
            ViewData["courseId"] = courseId;

            return View(enrollments.ToList());
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherCourseEdit(int? enrollmentId, int teacherId)
        {
            if (enrollmentId == null)
            {
                return NotFound();
            }

            Enrollment enrollment = _context.Enrollment.Include(e => e.Course).Include(e => e.Student).FirstOrDefault(e => e.Id == enrollmentId);
            Teacher teacher = _context.Teacher.FirstOrDefault(t => t.Id == teacherId);

            if (teacher == null || enrollment == null)
                return NotFound();

            // get logged in user
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            // check if logged in user has access
            if ((teacher.Id != currUser.TeacherId) || (enrollment.Course.FirstTeacherId != teacherId && enrollment.Course.SecondTeacherId != teacherId))
                return LocalRedirect("/Identity/Account/AccessDenied");

            // explicit loading
            //_context.Entry(enrollment).Reference(e => e.Course).Load();
            //_context.Entry(enrollment).Reference(e => e.Student).Load();

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);
            ViewData["teacherId"] = teacherId;

            return View(enrollment);
        }

        // POST: Enrollments/TeacherCourseEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherCourseEdit([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminarUrl,ProjectUrl,ExamPoints,SeminarPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment, int teacherId)
        {
            Teacher teacher = _context.Teacher.FirstOrDefault(t => t.Id == teacherId);

            if (teacher == null || enrollment == null)
                return NotFound();

            // get logged in user
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            // check if logged in user has access
            if ((teacher.Id != currUser.TeacherId) || (enrollment.Course.FirstTeacherId != teacherId && enrollment.Course.SecondTeacherId != teacherId))
                return LocalRedirect("/Identity/Account/AccessDenied");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(TeacherCourseIndex), new { courseId = enrollment.CourseId, teacherId = teacherId });
            }

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);
            ViewData["teacherId"] = teacherId;

            return View(enrollment);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentIndex(int studentId)
        {
            Student student = _context.Student.FirstOrDefault(t => t.Id == studentId);

            if (student == null)
                return NotFound();

            // get logged in user
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            // check if logged in user has access
            if (student.Id != currUser.StudentId)
                return LocalRedirect("/Identity/Account/AccessDenied");

            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();

            enrollments = enrollments.Where(e => e.StudentId == studentId);
            enrollments = enrollments.Include(e => e.Student).Include(e => e.Course);

            ViewData["studentId"] = studentId;

            return View(enrollments.ToList());
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentEdit(int? enrollmentId, int studentId)
        {
            if (enrollmentId == null)
            {
                return NotFound();
            }

            Enrollment enrollment = await _context.Enrollment.FindAsync(enrollmentId);
            Student student = _context.Student.FirstOrDefault(t => t.Id == studentId);

            if (student == null || enrollment == null)
                return NotFound();

            // get logged in user
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            // check if logged in user has access
            if (student.Id != currUser.StudentId || student.Id != enrollment.StudentId)
                return LocalRedirect("/Identity/Account/AccessDenied");

            // explicit loading
            _context.Entry(enrollment).Reference(e => e.Course).Load();
            _context.Entry(enrollment).Reference(e => e.Student).Load();

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);
            ViewData["studentId"] = studentId;

            EnrollmentUploadViewModel viewModel = new EnrollmentUploadViewModel
            {
                Enrollment = enrollment,
                SeminarFile = null
            };

            return View(viewModel);
        }

        // POST: Enrollments/StudentEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentEdit(EnrollmentUploadViewModel viewModel, int studentId)
        {
            Student student = _context.Student.FirstOrDefault(t => t.Id == studentId);

            if (student == null || viewModel.Enrollment == null)
                return NotFound();

            // get logged in user
            MVCWorkshopUser currUser = await userManager.GetUserAsync(User);

            // check if logged in user has access
            if (student.Id != currUser.StudentId || student.Id != viewModel.Enrollment.StudentId)
                return LocalRedirect("/Identity/Account/AccessDenied");

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.SeminarFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewModel);
                        viewModel.Enrollment.SeminarUrl = uniqueFileName;
                    }

                    _context.Update(viewModel.Enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(viewModel.Enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(StudentIndex), new { studentId = studentId });
            }

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", viewModel.Enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", viewModel.Enrollment.StudentId);
            ViewData["studentId"] = studentId;

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminCreate(int? courseId)
        {
            ViewData["CourseList"] = new SelectList(_context.Course.OrderBy(c => c.Title), "Id", "Title", courseId);
            ViewData["StudentList"] = new SelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullNameWithId");

            return View();
        }

        // POST: Enrollments/AdminCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminCreate(EnrollmentsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Enrollment enrollment;

                foreach (var sId in viewModel.StudentIds)
                {
                    enrollment = await _context.Enrollment
                        .FirstOrDefaultAsync(e => e.StudentId == sId && e.CourseId == viewModel.CourseId &&
                        e.Year == viewModel.Year && e.Semester == viewModel.Semester);

                    if (enrollment == null)
                    {
                        enrollment = new Enrollment
                        {
                            StudentId = sId,
                            CourseId = viewModel.CourseId,
                            Year = viewModel.Year,
                            Semester = viewModel.Semester
                        };

                        _context.Add(enrollment);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseList"] = new SelectList(_context.Course.OrderBy(c => c.Title), "Id", "Title", viewModel.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullNameWithId", viewModel.StudentIds);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);

            return View(enrollment);
        }

        // POST: Enrollments/AdminEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEdit(int id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminarUrl,ProjectUrl,ExamPoints,SeminarPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
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

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentList"] = new SelectList(_context.Student, "Id", "FullNameWithId", enrollment.StudentId);

            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEditMultiple(int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }

            var students = _context.Enrollment.Where(e => e.CourseId == courseId && e.FinishDate == null).Include(e => e.Student).Select(e => e.Student);

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", courseId);
            ViewData["StudentList"] = new SelectList(students, "Id", "FullNameWithId");

            return View();
        }

        // POST: Enrollments/AdminEditMultiple/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEditMultiple(UnenrollMultipleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Enrollment enrollment;

                foreach (var sId in viewModel.StudentIds)
                {
                    enrollment = await _context.Enrollment
                        .FirstOrDefaultAsync(e => e.StudentId == sId && e.CourseId == viewModel.CourseId);

                    if (enrollment != null)
                    {
                        enrollment.FinishDate = viewModel.FinishDate;

                        _context.Update(enrollment);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Courses");
            }

            var students = _context.Enrollment.Where(e => e.CourseId == viewModel.CourseId && e.FinishDate == null).Include(e => e.Student).Select(e => e.Student);

            ViewData["CourseList"] = new SelectList(_context.Course, "Id", "Title", viewModel.CourseId);
            ViewData["StudentList"] = new SelectList(students, "Id", "FullNameWithId");

            return View();
        }

        private string UploadedFile(EnrollmentUploadViewModel model)
        {
            string uniqueFileName = null;

            if (model.SeminarFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "documents");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.SeminarFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SeminarFile.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
