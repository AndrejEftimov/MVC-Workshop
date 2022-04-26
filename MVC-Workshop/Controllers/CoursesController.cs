﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Data;
using MVC_Workshop.Models;

namespace MVC_Workshop.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MVCWorkshopContext _context;

        public CoursesController(MVCWorkshopContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string title, int semester, string programme)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();

            if (!String.IsNullOrEmpty(title))
            {
                courses = courses.Where(c => c.Title.Contains(title));
            }

            if (semester >= 1)
            {
                courses = courses.Where(c => c.Semester == semester);
            }

            if (!String.IsNullOrEmpty(programme))
            {
                courses = courses.Where(c => c.Programme.Contains(programme));
            }

            courses = courses.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher);

            return View(courses.ToList());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["FirstTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName");
            ViewData["SecondTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName");

            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["FirstTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            ViewData["FirstTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);

            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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

            ViewData["FirstTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherList"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);

            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
