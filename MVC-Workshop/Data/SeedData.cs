using Microsoft.EntityFrameworkCore;
using MVC_Workshop.Models;

namespace MVC_Workshop.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCWorkshopContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<MVCWorkshopContext>>()))
            {
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
