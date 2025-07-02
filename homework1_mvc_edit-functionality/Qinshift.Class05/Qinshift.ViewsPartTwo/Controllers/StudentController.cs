using Microsoft.AspNetCore.Mvc;
using Qinshift.ViewsPartTwo.Database;
using Qinshift.ViewsPartTwo.Models.Domain;
using Qinshift.ViewsPartTwo.Models.ViewModels;

namespace Qinshift.ViewsPartTwo.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index(string search = null)
        {
            // Bonus search functionality
            var students = InMemoryDb.Students
                .Where(student =>
                    string.IsNullOrWhiteSpace(search) ||
                    student.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) || // case-insensitive search
                    student.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (student.ActiveCourse?.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) // prevent null exception
                )
                .Select(student => new StudentViewModel
                {
                    Id = student.Id,
                    FullName = student.GetFullName(),
                    Age = DateTime.UtcNow.Year - student.DateOfBirth.Year,
                    ActiveCourseName = student.ActiveCourse?.Name ?? "No Course" // display if found or "No Course" if null
                })
                .ToList();

            ViewBag.WelcomeMessage = "Welcome to the Student Management System";
            ViewBag.SearchQuery = search;

            return View(students);
        }

        // /student/create
        public IActionResult Create()
        {
            CreateStudentViewModel createStudentViewModel = new();
            createStudentViewModel.Courses = InMemoryDb.Courses.Select(c => new CourseOptionViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return View(createStudentViewModel);
        }

        [HttpPost]
        public IActionResult Create(CreateStudentViewModel model)
        {
            var student = new Student
            {
                Id = InMemoryDb.Students.Max(s => s.Id) + 1,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                ActiveCourse = InMemoryDb.Courses.FirstOrDefault(c => c.Id == model.ActiveCourseId)
            };

            InMemoryDb.Students.Add(student);

            TempData["FormMessage"] = "Student succesfully created!";

            return RedirectToAction("Index");
        }

        // HOMEWORK :)
        [HttpGet]
        public IActionResult Edit(int studentId)
        {
            var student = InMemoryDb.Students.FirstOrDefault(t => t.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new EditStudentViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                ActiveCourseId = student.ActiveCourse?.Id, // get the ActiveCourse Id if it exists, otherwise set to null
                Courses = InMemoryDb.Courses.Select(c => new CourseOptionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditStudentViewModel model)
        {
            var student = InMemoryDb.Students.FirstOrDefault(t => t.Id == model.Id);
            if (student == null)
            {
                return NotFound();
            }

            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.DateOfBirth = model.DateOfBirth;

            // avoid null reference exceptions
            student.ActiveCourse = model.ActiveCourseId.HasValue
                ? InMemoryDb.Courses.FirstOrDefault(c => c.Id == model.ActiveCourseId.Value)
                : null;

            TempData["FormMessage"] = "Successfully updated.";
            return RedirectToAction("Index");
        }

        // Bonus delete
        [HttpPost]
        public IActionResult Delete(int studentId)
        {
            var student = InMemoryDb.Students.FirstOrDefault(t => t.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }

            InMemoryDb.Students.Remove(student);
            TempData["FormMessage"] = $"Student '{student.GetFullName()}' deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}