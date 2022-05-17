using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Areas.Identity.Data;
using MVC_Workshop.Models;

namespace MVC_Workshop.Data
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MVCWorkshopUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            roleCheck = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Student")); }
            roleCheck = await RoleManager.RoleExistsAsync("Teacher");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Teacher")); }

            MVCWorkshopUser user = await UserManager.FindByEmailAsync("admin@mvcworkshop.com");
            if (user == null)
            {
                var User = new MVCWorkshopUser();
                User.Email = "admin@mvcworkshop.com";
                User.UserName = "admin@mvcworkshop.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCWorkshopContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<MVCWorkshopContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Student.Any() || context.Teacher.Any() || context.Course.Any() || context.Enrollment.Any())
                {
                    return; // DB contains data
                }

                context.Student.AddRange(
                    new Student { StudentId = "256/2017", FirstName = "Keira", LastName = "Knightley", EnrollmentDate = DateTime.Parse("2017-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "264/2019", FirstName = "Scarlett", LastName = "Johansson", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "189/2019", FirstName = "Emily", LastName = "Blunt", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "75/2019", FirstName = "Ryan", LastName = "Gosling", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "239/2017", FirstName = "Jodie", LastName = "Foster", EnrollmentDate = DateTime.Parse("2017-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "138/2019", FirstName = "Keanu", LastName = "Reeves", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "47/2019", FirstName = "Emma", LastName = "Stone", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "98/2019", FirstName = "Tom", LastName = "Cruise", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "83/2017", FirstName = "Chris", LastName = "Pratt", EnrollmentDate = DateTime.Parse("2017-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" },
                    new Student { StudentId = "278/2019", FirstName = "Will", LastName = "Smith", EnrollmentDate = DateTime.Parse("2019-9-1"), CurrentSemester = 6, EducationLevel = "Graduate" }
                    );

                context.SaveChanges();

                context.Teacher.AddRange(
                    new Teacher { FirstName = "Harrison", LastName = "Ford", Degree = "PhD", AcademicRank = "Associate Professor", OfficeNumber = "09835270", HireDate = DateTime.Parse("2001-01-01") },
                    new Teacher { FirstName = "Matthew", LastName = "McConaughey", Degree = "PhD", AcademicRank = "Full Professor", OfficeNumber = "19250370", HireDate = DateTime.Parse("2017-01-01") }
                    );

                context.SaveChanges();

                context.Course.AddRange(
                    new Course { Title = "Development of serverside WEB apps", Credits = 6, Semester = 6, Programme = "KTI, TKII", EducationLevel = "Graduate", FirstTeacherId = 2, SecondTeacherId = 1 },
                    new Course { Title = "Frontend WEB development", Credits = 6, Semester = 6, Programme = "KTI, TKII", EducationLevel = "Graduate", FirstTeacherId = 2, SecondTeacherId = 1 }
                    );

                context.SaveChanges();

                context.Enrollment.AddRange(
                    new Enrollment { CourseId = 1, StudentId = 1, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 2, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 3, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 4, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 5, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 6, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 7, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 1, StudentId = 8, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 2, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 5, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 6, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 7, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 8, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 9, Year = 2022, Semester = "Summer" },
                    new Enrollment { CourseId = 2, StudentId = 10, Year = 2022, Semester = "Summer" }
                    );

                context.SaveChanges();
            }
        }
    }
}
