using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Models;
using MVC_Workshop.Areas.Identity.Data;

namespace MVC_Workshop.Data
{
    public class MVCWorkshopContext : IdentityDbContext<MVCWorkshopUser>
    {
        public MVCWorkshopContext(DbContextOptions<MVCWorkshopContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }

        public DbSet<Enrollment> Enrollment { get; set; }

        public DbSet<Teacher> Teacher { get; set; }

        public DbSet<Course> Course { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Enrollment>()
                .HasOne<Student>(a => a.Student)
                .WithMany(a => a.Enrollments)
                .HasForeignKey(a => a.StudentId);

            builder.Entity<Enrollment>()
                .HasOne<Course>(a => a.Course)
                .WithMany(a => a.Enrollments)
                .HasForeignKey(a => a.CourseId);

            builder.Entity<Course>()
                .HasOne<Teacher>(a => a.FirstTeacher)
                .WithMany(a => a.CoursesFirst)
                .HasForeignKey(a => a.FirstTeacherId);

            builder.Entity<Course>()
                .HasOne<Teacher>(a => a.SecondTeacher)
                .WithMany(a => a.CoursesSecond)
                .HasForeignKey(a => a.SecondTeacherId);
        }
    }
}
