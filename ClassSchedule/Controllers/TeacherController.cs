using Microsoft.AspNetCore.Mvc;
using ClassSchedule.Models;

namespace ClassSchedule.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IRepository<Teacher> _teacherRepository;

        public TeacherController(IRepository<Teacher> teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public ViewResult Index()
        {
            var options = new QueryOptions<Teacher>
            {
                OrderBy = t => t.LastName
            };

            var teachers = _teacherRepository.List(options);
            return View(teachers);
        }

        [HttpGet]
        public ViewResult Add() => View();

        [HttpPost]
        public IActionResult Add(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _teacherRepository.Insert(teacher);
                _teacherRepository.Save();
                TempData["msg"] = $"{teacher.FullName} added to list of teachers";
                return RedirectToAction("Index");
            }
            else
            {
                return View(teacher);
            }
        }

        [HttpGet]
        public ViewResult Delete(int id)
        {
            var teacher = _teacherRepository.Get(id);
            return View(teacher);
        }

        [HttpPost]
        public RedirectToActionResult Delete(Teacher teacher)
        {
            teacher = _teacherRepository.Get(teacher.TeacherId); // to get teacher name for notification message
            _teacherRepository.Delete(teacher);
            _teacherRepository.Save();
            TempData["msg"] = $"{teacher.FullName} removed from list of teachers";
            return RedirectToAction("Index");
        }
    }
}